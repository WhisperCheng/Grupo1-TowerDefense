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

    public PlayerInput playerInput;

    public GameObject marcador;
    [Header("Partículas de construcción")]
    public GameObject particulasConstruccion;

    private static Tower torre;
    private Button currentButton;
    //public Tower torreCopiada;

    public bool objetoSiendoArrastrado = false;
    public bool bloqueoDisparo = false;
    public Color selectedColor;
    public float maxPlaceDistance;

    GameObject particulasCopia;
    
    List<Material[]> materialesTorre;
    List<Color[]> coloresOriginalesTorre;

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
        playerInput.actions.FindActionMap("Player").Enable();
        playerInput.actions.FindActionMap("UI").Enable(); // por defecto está desactivado ya que solo
        // se puede por defecto tener un action map activado a la vez, pero se puede bypasear
        // haciendo esto
        //https://youtu.be/NZBAr_V7r0M?t=153
        Cursor.lockState = CursorLockMode.Locked;
        materialesTorre = new List<Material[]>();
        coloresOriginalesTorre = new List<Color[]>();
    }

    // Update is called once per frame
    void Update()
    {
        ManageTowerPlacement();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        /*if (objetoCopiado != null)
        {
            Destroy(objetoCopiado);
        }*/
        GenerateTower(torre);
    }

    public void GenerateTower(Tower objeto)
    {
        if (!objetoSiendoArrastrado) // Para que solo se pueda generar un objeto al mismo tiempo
                                     // hasta que no se coloque
        {
            //Ray rayo = Camera.main.ScreenPointToRay(marcador.transform.position);
            //RaycastHit golpeRayo;
            /*bool colisionConRayo = Physics.Raycast(rayo, out golpeRayo, maxPlaceDistance, ~GameManager.Instance.layerJugador
                | ~GameManager.Instance.layerUI);*/
            /*torreCopiada = Instantiate(objeto,
                    !colisionConRayo ? objeto.transform.position : golpeRayo.point, objeto.transform.rotation);*/
            torre = torre.GetComponent<IPoolable>().GetFromPool().GetComponent<Tower>();
            // Se cambia el "tag" <<original>> del objeto a falso para posteriormente poder borrar todos
            // excepto el original
            //objetoCopiado.GetComponent<PlaceableObject>().setIsACopy(true);
            // TODO: Sin uso, pero se queda así por si hace falta luego eliminar todas las copias

            // Esto es para que al colocarlo no se buguee con el raycast todo el rato, hasta que se termine de colocar
            ToggleTowerCollisions(torre, false);
            SetPreviewMode(true);
            bloqueoDisparo = true;
            objetoSiendoArrastrado = true;
        }
    }

    private IEnumerator DesbloquearDisparo() //Para que no dispare cuando va a colocar
    {
        yield return null; 
        bloqueoDisparo = false;
    }

    void SetPreviewMode(bool previewMode)
    {
        // Por cada gameObject se añade su array de materiales correspondiente a la lista y la copia
        // de los materiales originales
        Transform[] childs = torre.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            Transform child = childs[i];
            Material[] mats = new Material[0];
            Color[] matsColorCopy = null;

            Renderer rend = child.gameObject.GetComponent<Renderer>();
            // PREPARACIÓN: Si está en modo preview y el objeto tiene un renderes disponible, se preparan
            // los materiales del objeto y un array para los nuevos colores
            if (rend != null)
            {
                if (previewMode)
                {
                    mats = rend.materials;
                    matsColorCopy = new Color[mats.Length];
                }
                else
                { // De lo contrario, se referencian los ya definidos materiales del objeto sin necesidad de volverlo a hacer
                    //if (materialesTorre.Count > 0)
                    //{
                    mats = materialesTorre[i];
                    //}
                }

                // Por cada material, se gestiona su color correspondiente según el modo preview
                for (int j = 0; j < mats.Length; j++)
                {
                    Color c;
                    float transparency;
                    if (previewMode)
                    {
                        // ALMACENAMIENTO: Se escoge el color de cada material para sobre la marcha almacenarlo en la lista
                        // de colores originales por cada array de materiales y cambiar el color del objeto
                        c = new Color(selectedColor.r, selectedColor.g, selectedColor.b, 1);
                        matsColorCopy[j] = mats[j].color; // Almacenando el color original
                    }
                    else
                    {
                        // RESUSTITUCIÓN: Se recupera el color original de cada material correspondiente para volverlo a aplicar
                        // al objeto implicado
                        c = coloresOriginalesTorre[i][j];
                    }
                    Material m = mats[j]; // Referenciando el material actual

                    // CAMBIO DE COLOR Y TRANSPARENCIA
                    m.SetColor("_BaseColor", c);
                    m.SetColor("_Color", c); // por si el material no tiene el toon shader
                    transparency = previewMode ? selectedColor.a : c.a;
                    m.SetFloat("_Tweak_transparency", -(1 - transparency));
                    // Sombras
                    if (m.HasColor("_1st_ShadeColor") && m.HasColor("_2nd_ShadeColor"))
                    {
                        m.SetColor("_1st_ShadeColor", c);
                        m.SetColor("_2nd_ShadeColor", c);
                    }
                }
                if (previewMode) // Si está en preview se populan las listas para su futuro uso (previewMode == false)
                {
                    materialesTorre.Add(mats);
                    coloresOriginalesTorre.Add(matsColorCopy);
                }
            }
        }
        if (!previewMode) // Finalmente, si deja de estar en modo preview, se limpian las listas para un posterior nuevo uso
        { // (al salir de todos los bucles)
            ClearSelectedObjInfo();
            torre.UnlockTower(); // Desbloquea la torre, permite atacar y rotar hacia los enemigos
            //MoneyManager.
        }
    }

    /// <summary>
    /// Vacía la información de las listas usadas para posteriormente usar correctamente las listas con nuevos objetos.
    /// </summary>
    // Se invoca al terminar de colocar un objeto o al cancelar la colocación de este (click derecho)
    private void ClearSelectedObjInfo()
    {
        materialesTorre = new List<Material[]>();
        coloresOriginalesTorre = new List<Color[]>();
    }

    private void ManageTowerPlacement()
    {
        if (objetoSiendoArrastrado)
        {
            Ray rayo = Camera.main.ScreenPointToRay(marcador.transform.position);
            RaycastHit golpeRayo;
            bool colisionConRayo = Physics.Raycast(rayo, out golpeRayo, maxPlaceDistance, 1 << GameManager.Instance.layerPath);
            if (colisionConRayo)
            {
                if (!torre.gameObject.activeSelf)
                {
                    torre.gameObject.SetActive(true);
                }
                torre.gameObject.transform.position = golpeRayo.point;

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
    
    public void OnClickButton(Button btn)
    {
        currentButton = btn;
    }

    public void OnClickButtons(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (objetoSiendoArrastrado) // si ya se está arrastrando se cancela la colocación
            {
                ReturnInstanceCopy();
            }
            else
            {
                currentButton.Select();
                currentButton.onClick.Invoke();
                
                if (!GameUIManager.Instance.activeBuildUI) // si el menú está escondido, mostrarlo
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

    public void OnClickPlaceTower(InputAction.CallbackContext ctx)
    {
        if (objetoSiendoArrastrado && ctx.performed)
        {
            if (!torre.gameObject.activeSelf)
            {
                ReturnInstanceCopy();
                return;
            }
            particulasCopia = Instantiate(particulasConstruccion);
            particulasCopia.transform.position = torre.transform.position;
            particulasCopia.GetComponent<ParticleSystem>().Play();

            ToggleTowerCollisions(torre, true);
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
        }
    }
    public void OnRightClickPlaceTower(InputAction.CallbackContext ctx)
    {
        if (objetoSiendoArrastrado && ctx.performed)
        {
            EventSystem.current.SetSelectedGameObject(null); // deseleccionar boton
            ReturnInstanceCopy();
        }
        // TODO: Devolver a la objectpool
    }

    private void ReturnInstanceCopy()
    {
        //torre.gameObject.SetActive(true); // Necesario para activar los renderers
        SetPreviewMode(false);
        ToggleTowerCollisions(torre, true);
        ClearSelectedObjInfo();
        objetoSiendoArrastrado = false;
        StartCoroutine(DesbloquearDisparo());
        //Destroy(torreCopiada);
        torre.GetComponent<IPoolable>().ReturnToPool();
    }

    public void DesignMainTower(Tower tower)
    {
        torre = tower;
    }

    public Tower GetCurrentManagedTower()
    {
        return torre;
    }
}
