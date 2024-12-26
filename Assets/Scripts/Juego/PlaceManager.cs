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
    [Header("Part�culas de construcci�n")]
    public GameObject particulasConstruccion;

    private static Tower torre;
    private Button currentButton;
    //public Tower torreCopiada;

    public bool objetoSiendoArrastrado = false;
    public bool bloqueoDisparo = false;
    public Color selectedColor;
    public float maxPlaceDistance;

    GameObject particulasCopia;

    MaterialPropertyBlock materialesSeleccionados;

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
        playerInput.actions.FindActionMap("UI").Enable(); // por defecto est� desactivado ya que solo
        // se puede por defecto tener un action map activado a la vez, pero se puede bypasear
        // haciendo esto
        //https://youtu.be/NZBAr_V7r0M?t=153
        Cursor.lockState = CursorLockMode.Locked;

        // Materiales Seleccionados
        materialesSeleccionados = new MaterialPropertyBlock();
        materialesSeleccionados.SetColor("_BaseColor", selectedColor);
        materialesSeleccionados.SetColor("_Color", selectedColor); // por si el material no tiene el toon shader
        materialesSeleccionados.SetFloat("_Tweak_transparency", -(1 - selectedColor.a));
        // Sombras
        materialesSeleccionados.SetColor("_1st_ShadeColor", selectedColor);
        materialesSeleccionados.SetColor("_2nd_ShadeColor", selectedColor);
    }

    // Update is called once per frame
    void Update()
    {
        ManageTowerPlacement();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GenerateTower();
    }

    public void GenerateTower() // Genera
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
        Transform[] childs = torre.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            Transform child = childs[i];
            Renderer rend = child.gameObject.GetComponent<Renderer>();
            if (rend != null)
            {
                if (previewMode) // Si se est� en modo selecci�n, se cambia el PropertyBlock de materiales al de materiales seleccionados
                {
                    rend.SetPropertyBlock(materialesSeleccionados);
                }
                else // Y si deja de estarlo, se quita el PropertyBlock de materiales seleccionados
                {
                    rend.SetPropertyBlock(null);
                }
            }
        }

        if (!previewMode) // Finalmente, si deja de estar en modo preview, se desbloquea la torre,
        {  // lo que permite atacar y rotar hacia los enemigos
            torre.UnlockTower();
        }
    }

    private void ManageTowerPlacement()
    {
        if (objetoSiendoArrastrado)
        {
            Ray rayo = Camera.main.ScreenPointToRay(marcador.transform.position);
            RaycastHit golpeRayo;
            bool colisionConRayo = Physics.Raycast(rayo, out golpeRayo, maxPlaceDistance, (1 << GameManager.Instance.layerTerreno));
            if (colisionConRayo)
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
    
    public void OnClickButton(Button btn)
    {
        currentButton = btn;
    }

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
