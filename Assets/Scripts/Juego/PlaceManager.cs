using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class PlaceManager : MonoBehaviour
{
    public static PlaceManager Instance { get; private set; }

    public GameObject marcador;
    [Header("Partículas de construcción")]
    public ParticleSystem particulasConstruccion;
    public GameObject particlesParent;

    private static Tower torre;
    private Button currentButton;

    public bool objetoSiendoArrastrado = false;
    public bool bloqueoDisparo = false;
    public Color selectionColor;
    public Color invalidSelectionColor;
    public float maxPlaceDistance;

    protected float _currentSellTime;
    protected bool _canPlaceTower = false;

    //GameObject particulasCopia;

    MaterialPropertyBlock m_SeleccionPropertyBlock;
    MaterialPropertyBlock m_SeleccionInvalidaPropertyBlock;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.playerControls.actions.FindActionMap("Player").Enable();
        GameManager.Instance.playerControls.actions.FindActionMap("UI").Enable(); // por defecto está desactivado ya que solo
        // se puede por defecto tener un action map activado a la vez, pero se puede bypasear
        // haciendo esto
        //https://youtu.be/NZBAr_V7r0M?t=153
        Cursor.lockState = CursorLockMode.Locked;

        // Materiales Seleccionados
        m_SeleccionPropertyBlock = ColorUtils.CreateToonShaderPropertyBlock(selectionColor);
        m_SeleccionPropertyBlock.SetFloat("_Tweak_transparency", -(1 - selectionColor.a));  // Modificar transparencia


        // Materiales invalidos
        m_SeleccionInvalidaPropertyBlock = ColorUtils.CreateToonShaderPropertyBlock(invalidSelectionColor);
        m_SeleccionInvalidaPropertyBlock.SetFloat("_Tweak_transparency", -(1 - selectionColor.a)); // Modificar transparencia
    }

    // Update is called once per frame
    void Update()
    {
        ManageTowerPlacement();
    }

    public void OnPointerClick(PointerEventData eventData) { GenerateTower(); }

    public void GenerateTower() // Genera el tipo de torre designada
    {
        //Debug.Log(MoneyManager.Instance.gems);
        if (MoneyManager.Instance.gems >= torre.Money)
        {
            if (!objetoSiendoArrastrado) // Para que solo se pueda generar un objeto al mismo tiempo
            {                                                               // hasta que no se coloque
                torre = torre.GetComponent<IPoolable>().GetFromPool().GetComponent<Tower>();
                ManageTowerPlacement();
                // Esto es para que al colocarlo no se buguee con el raycast todo el rato, hasta que se termine de colocar
                ToggleTowerCollisions(torre, false);
                SetPreviewMode(true);
                bloqueoDisparo = true;
                objetoSiendoArrastrado = true;
            }
        }
    }

    private IEnumerator DesbloquearDisparo() //Para que no dispare cuando va a colocar
    {
        yield return null;
        bloqueoDisparo = false;
    }

    private void SetPreviewMode(bool previewMode)
    {
        ColorUtils.ChangeObjectMaterialColors(torre.gameObject, m_SeleccionPropertyBlock, previewMode);

        if (!previewMode && torre.placed) // Finalmente, si deja de estar en modo preview, se desbloquea la torre,
        { torre.UnlockTower(); } // lo que permite atacar y rotar hacia los enemigos
    }

    private void UpdateCurrentMode(MaterialPropertyBlock m_propertyBlock, bool updateCanBePlacedStatus)
    {
        _canPlaceTower = updateCanBePlacedStatus;
        ColorUtils.ChangeObjectMaterialColors(torre.gameObject, m_propertyBlock, true);
    }

    public void RotateCurrentTower(InputAction.CallbackContext context)
    {
        if (context.performed && objetoSiendoArrastrado && torre != null && torre.isActiveAndEnabled
            && !LeanTween.isTweening(torre.gameObject)) // Solo se activa si se está arrastrando el objeto activo y no
        {                                                                   // se está reproduciendo animación de rotar
            //torre.transform.LeanRotateY(torre.transform.eulerAngles.y + 90,0.25f);
            LeanTween.rotateY(torre.gameObject, torre.transform.eulerAngles.y + 90, 0.25f);
        }
    }

    // ==== Colocación y manejo de torres ====
    private void ManageTowerPlacement()
    {
        if (objetoSiendoArrastrado)
        {
            Ray rayo = Camera.main.ScreenPointToRay(marcador.transform.position);
            RaycastHit golpeRayo;
            int terrainMask = 1 << GameManager.Instance.layerTerreno; // Detecta todo el terreno/suelo
            int areaDecoMask = 1 << GameManager.Instance.layerAreaDeco; // Detecta los bordes de fuera del camino
            int pathBordersMask = 1 << GameManager.Instance.layerPathBordes; // Detecta obstáculos de decoración
            int towersMask = 1 << GameManager.Instance.layerTorres | 1 << GameManager.Instance.layerTrap; // Detecta las torres cercanas
            int pathMask = 1 << GameManager.Instance.layerPath; // Detecta solo los caminos por donde pasan los enemigos
            //int bordersMask = 1 << GameManager.Instance.layerBordes;

            bool validCollision = false;
            bool colisionConRayo = false;
            if (torre.CompareTag(GameManager.Instance.tagTorresCamino)) // Si es una torre de camino
            {
                Collider[] outsidePathCols = null;
                colisionConRayo = Physics.Raycast(rayo, out golpeRayo, maxPlaceDistance, pathMask | terrainMask);
                // Para hacer que funcione si fuera necesario en zonas donde el path está enterrado ligeramente, usar RayCastAll
                if (colisionConRayo)
                {
                    // Si se está apuntando a un camino, podrá darse la posibilidad de que si se pueda colocar si no está cerca
                    // de un borde u otra cosa, pero si se está apuntando al terreno entonces outsidePathCols será nulo
                    // y la colisión no será válida, aunque aún así como colisionConRayo estará a true servirá para
                    // mostrar la planta de color rojo en lugar de hacer que desaparezca e indicar que no se puede poner
                    if (golpeRayo.transform.gameObject.layer == GameManager.Instance.layerPath)
                        outsidePathCols = Physics.OverlapSphere(golpeRayo.point, torre.GetTowerRadiusSize(), pathBordersMask);
                }

                // Debug.Log(colisionConRayo + "-" + outsidePathCols + "-" + outsidePathCols?.Length);
                validCollision = colisionConRayo && outsidePathCols != null && outsidePathCols.Length == 0;
                // Si el "tamaño" de la torre no registra ningún borde exterior de camino dentro de su área, se puede colocar
            }
            else // Si no es una torre de camino
            {
                Collider[] pathCols = null;
                colisionConRayo = Physics.Raycast(rayo, out golpeRayo, maxPlaceDistance, terrainMask);
                if (colisionConRayo)
                {
                    pathCols = Physics.OverlapSphere(golpeRayo.point, torre.GetTowerRadiusSize(), pathMask | areaDecoMask);
                }

                bool isNotlimitAngle = Vector3.Angle(golpeRayo.normal, Vector3.up) <= 60; // Ángulo límite de pendiente
                validCollision = colisionConRayo && pathCols != null && pathCols.Length == 0 && isNotlimitAngle;
                // Si el "tamaño" de la torre no registra ningún camino dentro de su área, se puede colocar
            }

            Collider[] towersInsideSizeRadius = Physics.OverlapSphere(golpeRayo.point, torre.GetTowerRadiusSize(), towersMask);
            bool areTowersInsideSizeRadius = towersInsideSizeRadius.Length > 0;

            // Aquí es donde se actualiza la posición
            if (validCollision && !areTowersInsideSizeRadius) // Se comprueba si se puede colocar la torre
            {
                if (!torre.gameObject.activeSelf)
                {
                    torre.gameObject.SetActive(true);
                }
                torre.gameObject.transform.position = golpeRayo.point;
                torre.SetLoaded(true); // Se establece que la torre ya ha sido cargada en el mundo. Útil para vereficar posteriormente
                // si todos los componentes han sido cargados y poder volver a enviar a la torre a la pool con los componentes ya inicializados
                UpdateCurrentMode(m_SeleccionPropertyBlock, true);
            }
            else // Si la torre no se puede colocar entonces se pone en color rojo
            {

                if (colisionConRayo)
                {
                    if (!torre.gameObject.activeSelf)
                    {
                        torre.gameObject.SetActive(true);
                    }
                    torre.SetLoaded(true);
                    torre.gameObject.transform.position = golpeRayo.point;
                }
                else
                {
                    torre.gameObject.SetActive(false);
                }
                UpdateCurrentMode(m_SeleccionInvalidaPropertyBlock, false);
            }
        }
    }

    // ==== Manejo de los botones de las torres ====
    public void DiscardCurrentTower()
    {
        if (objetoSiendoArrastrado) // si ya se está arrastrando se cancela la colocación
        {
            ReturnInstanceCopy();
        }
    }

    public void InvokeCurrentButton()
    {
        if (objetoSiendoArrastrado) // si ya se está arrastrando se cancela la colocación
        {
            ReturnInstanceCopy();
        }
        else
        {
            //currentButton.Select();
            currentButton.onClick.Invoke();
        }
    }

    public void OnTriggerButton(Button btn) { currentButton = btn; }

    private void ToggleTowerCollisions(Tower tower, bool boolValue)
    {
        // Alternar Colliders
        Collider[] childrenColliders = tower.GetComponentsInChildren<Collider>();
        foreach (Collider col in childrenColliders)
        {
            if (col != null)
            {
                col.enabled = boolValue;
            }
        }

        // Alternar NavMeshObstacles
        NavMeshObstacle[] obstacleList = tower.GetComponents<NavMeshObstacle>();

        foreach (NavMeshObstacle obst in obstacleList)
        {
            if (obst != null)
            {
                obst.enabled = boolValue;
            }
        }
    }
    public ParticleSystem StartParticleGameObjEffect(ParticleSystem pSys, Vector3 position)
    {
        GameObject particulasCopia = Instantiate(pSys.gameObject);
        particulasCopia.transform.position = position;
        ParticleSystem pConstruccion = particulasCopia.GetComponent<ParticleSystem>();
        pConstruccion.Play();
        float destroyTime = pConstruccion.main.duration + pConstruccion.main.startLifetime.constant;
        Destroy(particulasCopia, destroyTime);
        return pConstruccion;
    }

    public Vector3 GetGameObjectCenter(GameObject gObj)
    {
        float lowestChild = 0;
        float highestChild = 0;
        foreach (Transform child in gObj.transform)
        {
            if (child.position.y < lowestChild)
            {
                lowestChild = child.position.y;
            }
            if (child.position.y > highestChild)
            {
                highestChild = child.position.y;
            }
        }
        float centerY = (lowestChild + highestChild) / 2;
        return gObj.transform.position + gObj.transform.up * centerY;
    }

    public void OnClickPlaceTower(InputAction.CallbackContext ctx)
    {
        if (objetoSiendoArrastrado && ctx.performed)
        {
            if (!_canPlaceTower)
            {
                ReturnInstanceCopy();
                return;
            }
            ManageTowerPlacement();
            ParticleSystem pSysConstruccion = StartParticleGameObjEffect(particulasConstruccion, torre.transform.position);
            pSysConstruccion.gameObject.transform.parent = particlesParent.transform; // Asignando padre

            ToggleTowerCollisions(torre, true);
            torre.placed = true;
            SetPreviewMode(false);
            objetoSiendoArrastrado = false;
            torre = null; // se "elimina" la referencia del objeto para que al hacer click derecho
                          // no se vuelva a eliminar
            StartCoroutine(DesbloquearDisparo());

            if (!GameUIManager.Instance.activeBuildUI)
            {
                GameUIManager.Instance.crossHead.SetActive(false);
            }
            EventSystem.current.SetSelectedGameObject(null); // deseleccionar boton

            //FMOD
            AudioManager.instance.PlayOneShot(FMODEvents.instance.buildPlant, this.transform.position);
        }
    }
    public void OnRightClickPlaceTower(InputAction.CallbackContext ctx)
    {
        if (objetoSiendoArrastrado && ctx.performed)
        {
            EventSystem.current.SetSelectedGameObject(null); // deseleccionar boton
            ReturnInstanceCopy();
        }
    }

    private void ReturnInstanceCopy()
    {
        torre.placed = false;
        SetPreviewMode(false);
        ToggleTowerCollisions(torre, true);
        objetoSiendoArrastrado = false;
        StartCoroutine(DesbloquearDisparo());
        torre.GetComponent<IPoolable>().ReturnToPool();
        torre = null;
    }

    public void DesignMainTower(Tower tower) { torre = tower; }

    public Tower GetCurrentManagedTower() { return torre; }
}
