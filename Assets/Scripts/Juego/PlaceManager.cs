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
    [Header("Part�culas de construcci�n")]
    public ParticleSystem particulasConstruccion;
    public GameObject particlesParent;

    private static Tower torre;
    private Button currentButton;

    public bool objetoSiendoArrastrado = false;
    public bool bloqueoDisparo = false;
    public Color selectionColor;
    public float maxPlaceDistance;

    protected float _currentSellTime;

    //GameObject particulasCopia;

    MaterialPropertyBlock materialesSeleccionPropertyBlock;

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
        GameManager.Instance.playerControls.actions.FindActionMap("UI").Enable(); // por defecto est� desactivado ya que solo
        // se puede por defecto tener un action map activado a la vez, pero se puede bypasear
        // haciendo esto
        //https://youtu.be/NZBAr_V7r0M?t=153
        //Cursor.lockState = CursorLockMode.Locked;

        // Materiales Seleccionados
        materialesSeleccionPropertyBlock = ColorUtils.CreateToonShaderPropertyBlock(selectionColor);
        materialesSeleccionPropertyBlock.SetFloat("_Tweak_transparency", -(1 - selectionColor.a));
        // Modificar transparencia
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
                                         // hasta que no se coloque
            {
                torre = torre.GetComponent<IPoolable>().GetFromPool().GetComponent<Tower>();

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
        ColorUtils.ChangeObjectMaterialColors(torre.gameObject, materialesSeleccionPropertyBlock, previewMode);

        if (!previewMode && torre.placed) // Finalmente, si deja de estar en modo preview, se desbloquea la torre,
        { torre.UnlockTower(); } // lo que permite atacar y rotar hacia los enemigos
    }

    public void RotateCurrentTower(InputAction.CallbackContext context)
    {
        if (context.performed && objetoSiendoArrastrado && torre != null && torre.isActiveAndEnabled
            && !LeanTween.isTweening(torre.gameObject)) // Solo se activa si se est� arrastrando el objeto activo y no
        {                                                                   // se est� reproduciendo animaci�n de rotar
            //torre.transform.LeanRotateY(torre.transform.eulerAngles.y + 90,0.25f);
            LeanTween.rotateY(torre.gameObject, torre.transform.eulerAngles.y + 90, 0.25f);
        }
    }

    private void ManageTowerPlacement()
    {
        if (objetoSiendoArrastrado)
        {
            Ray rayo = Camera.main.ScreenPointToRay(marcador.transform.position);
            RaycastHit golpeRayo;
            int terrainMask = 1 << GameManager.Instance.layerTerreno; // Detecta todo el terreno
            int areaDecoMask = 1 << GameManager.Instance.layerAreaDeco; // Detecta los bordes de fuera del camino u obst�culos de decoraci�n
            int pathMask = 1 << GameManager.Instance.layerPath; // Detecta solo los caminos por donde pasan los enemigos

            bool validCollision = false;
            bool colisionConRayo = false;
            if (torre.CompareTag(GameManager.Instance.tagTorresCamino))
            {
                Collider[] outsidePathCols = null;
                colisionConRayo = Physics.Raycast(rayo, out golpeRayo, maxPlaceDistance, pathMask);
                if (colisionConRayo)
                {
                    outsidePathCols = Physics.OverlapSphere(golpeRayo.point, torre.GetTowerRadiusSize(), areaDecoMask);
                }

                validCollision = (colisionConRayo && outsidePathCols != null && outsidePathCols.Length == 0) ? true : false;
                // Si el "tama�o" de la torre no registra ning�n borde exterior de camino dentro de su �rea, se puede colocar


            }
            else
            {
                Collider[] pathCols = null;
                colisionConRayo = Physics.Raycast(rayo, out golpeRayo, maxPlaceDistance, terrainMask);
                if (colisionConRayo)
                {
                    pathCols = Physics.OverlapSphere(golpeRayo.point, torre.GetTowerRadiusSize(), pathMask);
                }

                validCollision = (colisionConRayo && pathCols != null && pathCols.Length == 0) ? true : false;
                // Si el "tama�o" de la torre no registra ning�n camino dentro de su �rea, se puede colocar
            }

            //bool colisionConRayo = Physics.Raycast(rayo, out golpeRayo, maxPlaceDistance, terrainMask);
            //Debug.Log(validCollision);
            //if (validCollision)
            if (validCollision)
            {
                if (!torre.gameObject.activeSelf)
                {
                    torre.gameObject.SetActive(true);
                }
                torre.gameObject.transform.position = golpeRayo.point;
                torre.SetLoaded(true); // Se establece que la torre ya ha sido cargada en el mundo. �til para vereficar posteriormente
                // si todos los componentes han sido cargados y poder volver a enviar a la torre a la pool con los componentes ya inicializados
            }
            else
            {
                if (torre.gameObject.activeSelf)
                {
                    torre.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnClickButton(Button btn) { currentButton = btn; }

    public void OnClickButtons(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (objetoSiendoArrastrado) // si ya se est� arrastrando se cancela la colocaci�n
            {
                ReturnInstanceCopy();
            }
            else
            {
                currentButton.Select();
                currentButton.onClick.Invoke();

                if (!GameUIManager.Instance.activeBuildUI) // si el men� est� escondido, mostrarlo
                {
                    GameUIManager.Instance.ShowBuildUI(GameUIManager.Instance.menusTransitionTime);
                }
            }
        }
    }

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
            if (!torre.gameObject.activeSelf)
            {
                ReturnInstanceCopy();
                return;
            }

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
        SetPreviewMode(false);
        ToggleTowerCollisions(torre, true);
        objetoSiendoArrastrado = false;
        StartCoroutine(DesbloquearDisparo());
        //Destroy(torreCopiada);
        torre.GetComponent<IPoolable>().ReturnToPool();
    }

    public void DesignMainTower(Tower tower) { torre = tower; }

    public Tower GetCurrentManagedTower() { return torre; }
}
