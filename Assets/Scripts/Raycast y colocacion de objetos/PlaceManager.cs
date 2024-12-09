using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlaceManager : MonoBehaviour

{
    public static PlaceManager Instance { get; private set; }

    public PlayerInput playerInput;

    public GameObject marcador;
    public GameObject particulasConstruccion;

    private static GameObject torre;
    private Button currentButton;
    public GameObject torreCopiada;

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
        playerInput.actions.FindActionMap("UI").Enable(); // por defecto est� desactivado ya que solo
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

    public void GenerateTower(GameObject objeto)
    {
        if (!objetoSiendoArrastrado) // Para que solo se pueda generar un objeto al mismo tiempo
                                     // hasta que no se coloque
        {
            //Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray rayo = Camera.main.ScreenPointToRay(marcador.transform.position);
            RaycastHit golpeRayo;
            bool colisionConRayo = Physics.Raycast(rayo, out golpeRayo, maxPlaceDistance);
            torreCopiada = Instantiate(objeto,
                    !colisionConRayo ? objeto.transform.position : golpeRayo.point, objeto.transform.rotation);
            // Se cambia el "tag" <<original>> del objeto a falso para posteriormente poder borrar todos
            // excepto el original
            //objetoCopiado.GetComponent<PlaceableObject>().setIsACopy(true);
            // TODO: Sin uso, pero se queda as� por si hace falta luego eliminar todas las copias

            // Esto es para que al colocarlo no se buguee con el raycast todo el rato, hasta que se termine de colocar
            torreCopiada.GetComponent<BoxCollider>().enabled = false;
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
        // Por cada gameObject se a�ade su array de materiales correspondiente a la lista y la copia
        // de los materiales originales
        Transform[] childs = torreCopiada.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            Transform child = childs[i];
            Material[] mats = null;
            Color[] matsColorCopy = null;
            // PREPARACI�N: Si est� en modo preview, se preparan los materiales del objeto y un array para los nuevos colores
            if (previewMode)
            {
                mats = child.gameObject.GetComponent<Renderer>().materials;
                matsColorCopy = new Color[mats.Length];
            }
            else
            { // De lo contrario, se referencian los ya definidos materiales del objeto sin necesidad de volverlo a hacer
                mats = materialesTorre[i];
            }

            // Por cada material, se gestiona su color correspondiente seg�n el modo preview
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
                    // RESUSTITUCI�N: Se recupera el color original de cada material correspondiente para volverlo a aplicar
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
            if (previewMode) // Si est� en preview se populan las listas para su futuro uso (previewMode == false)
            {
                materialesTorre.Add(mats);
                coloresOriginalesTorre.Add(matsColorCopy);
            }
        }
        if (!previewMode) // Finalmente, si deja de estar en modo preview, se limpian las listas para un posterior nuevo uso
        { // (al salir de todos los bucles)
            ClearSelectedObjInfo();
        }
    }

    /// <summary>
    /// Vac�a la informaci�n de las listas usadas para posteriormente usar correctamente las listas con nuevos objetos.
    /// </summary>
    // Se invoca al terminar de colocar un objeto o al cancelar la colocaci�n de este (click derecho)
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
            if (Physics.Raycast(rayo, out golpeRayo, maxPlaceDistance))
            {
                if (!torreCopiada.activeSelf)
                {
                    torreCopiada.SetActive(true);
                }
                torreCopiada.gameObject.transform.position = golpeRayo.point;

            }
            else
            {
                if (torreCopiada.activeSelf)
                {
                    torreCopiada.SetActive(false);
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
                DestroyInstanceCopy();
            }
            else
            {
                currentButton.onClick.Invoke();
                currentButton.Select();
                if (!GameUIManager.Instance.activeBuildUI) // si el men� est� escondido, mostrarlo
                {
                    GameUIManager.Instance.ShowBuildUI(GameUIManager.Instance.menusTransitionTime);
                }
            }
        }
    }

    public void OnClickPlaceTower(InputAction.CallbackContext ctx)
    {
        if (objetoSiendoArrastrado && ctx.performed)
        {
            if (!torreCopiada.activeSelf)
            {
                DestroyInstanceCopy();
                return;
            }
            particulasCopia = Instantiate(particulasConstruccion);
            particulasCopia.transform.position = torreCopiada.transform.position;
            particulasCopia.GetComponent<ParticleSystem>().Play();

            torreCopiada.GetComponent<BoxCollider>().enabled = true;
            objetoSiendoArrastrado = false;
            SetPreviewMode(false);
            torreCopiada = null; // se "elimina" la referencia del objeto para que al hacer click derecho
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
        DestroyInstanceCopy();
        // TODO: Devolver a la objectpool
    }

    void DestroyInstanceCopy()
    {
        ClearSelectedObjInfo();
        objetoSiendoArrastrado = false;
        StartCoroutine(DesbloquearDisparo());
        Destroy(torreCopiada);
    }

    public void DesignMainTower(GameObject tower)
    {
        PlaceManager.torre = tower;
    }
}
