using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlaceManager : MonoBehaviour

{
    public static PlaceManager Instance { get; private set; }

    public Color selectedColor;
    public float maxPlaceDistance;
    public GameObject marcador;
    private static GameObject objeto;
    private static GameObject objetoCopiado;
    List<Material[]> materialesObjeto;
    List<Material[]> materialesOriginalesObjeto;
    //Color32 colorOriginalObjeto;
    private bool objetoSiendoArrastrado = false;
    public PlayerInput playerInput;

    public Button button1;
    public Button button3;

    Button currentButton;

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

        materialesObjeto = new List<Material[]>();
        materialesOriginalesObjeto = new List<Material[]>();
    }

    // Update is called once per frame
    void Update()
    {
        manageObjectPlacement();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        /*if (objetoCopiado != null)
        {
            Destroy(objetoCopiado);
        }*/
        generarObjeto(objeto);
    }

    public void generarObjeto(GameObject objeto)
    {
        if (!objetoSiendoArrastrado) // Para que solo se pueda generar un objeto al mismo tiempo
                                     // hasta que no se coloque
        {
            //Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray rayo = Camera.main.ScreenPointToRay(marcador.transform.position);
            RaycastHit golpeRayo;
            bool colisionConRayo = Physics.Raycast(rayo, out golpeRayo, maxPlaceDistance);
            objetoCopiado = Instantiate(objeto,
                    !colisionConRayo ? objeto.transform.position : golpeRayo.point, objeto.transform.rotation);
            // Se cambia el "tag" <<original>> del objeto a falso para posteriormente poder borrar todos
            // excepto el original
            //objetoCopiado.GetComponent<PlaceableObject>().setIsACopy(true);
            // TODO: Sin uso, pero se queda así por si hace falta luego eliminar todas las copias

            // Esto es para que al colocarlo no se buguee con el raycast todo el rato, hasta que se termine de colocar
            objetoCopiado.GetComponent<BoxCollider>().enabled = false;
            seleccionarObjeto();
            objetoSiendoArrastrado = true;
        }
    }

    void seleccionarObjeto()
    {
        // Por cada gameObject se añade su array de materiales correspondiente a la lista y la copia
        // de los materiales originales
        foreach (Transform child in objetoCopiado.GetComponentsInChildren<Transform>())
        {
            Material[] mats = child.gameObject.GetComponent<Renderer>().materials;
            Material[] matsCopy = new Material[mats.Length];
            for (int j = 0; j < mats.Length; j++)
            {
                Material m = mats[j];
                matsCopy[j] = new Material(m);
                // Importante poner new y crear un nuevo material, de lo
                // contrario el material seguirá vinculado al anterior

                m.SetColor("_BaseColor", new Color(selectedColor.r, selectedColor.g, selectedColor.b, 1));
                m.SetColor("_Color", new Color(selectedColor.r, selectedColor.g, selectedColor.b, 1));
                // por si el material no tiene el toon shader
                m.SetFloat("_Tweak_transparency", -(1 - selectedColor.a));

                //sombras
                if (m.HasColor("_1st_ShadeColor") && m.HasColor("_2nd_ShadeColor"))
                {
                    Color c = new Color(selectedColor.r, selectedColor.g, selectedColor.b, 1);
                    m.SetColor("_1st_ShadeColor", c);
                    m.SetColor("_2nd_ShadeColor", c);
                }
            }
            materialesObjeto.Add(mats);
            materialesOriginalesObjeto.Add(matsCopy);
        }
    }

    private void manageObjectPlacement()
    {
        if (objetoSiendoArrastrado)
        {
            //Vector3 point = Camera.main.WorldToScreenPoint(marcador.transform.position);
            Ray rayo = Camera.main.ScreenPointToRay(marcador.transform.position);
            RaycastHit golpeRayo;
            if (Physics.Raycast(rayo, out golpeRayo, maxPlaceDistance))
            {
                if (!objetoCopiado.activeSelf)
                {
                    objetoCopiado.SetActive(true);
                }
                objetoCopiado.gameObject.transform.position = golpeRayo.point;
            }
            else
            {
                if (objetoCopiado.activeSelf)
                {
                    objetoCopiado.SetActive(false);
                }
            }
            // onClickPlaceObj();
        }
    }

    public void onClickButton1()
    {
        currentButton = button1;
    }

    public void onClickButton3()
    {
        currentButton = button3;
    }

    public void onClickButtons(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (objetoSiendoArrastrado) // si ya se está arrastrando se cancela la colocación
            {
                destroyInstanceCopy();
            }
            else
            {
                currentButton.onClick.Invoke();
                currentButton.Select();
                if (!GameUIManager.Instance.activeObjectUI) // si el menú está escondido, mostrarlo
                {
                    GameUIManager.Instance.showObjectMenu(GameUIManager.Instance.menusTransitionTime);
                }
            }
        }
    }

    public void onClickPlaceObj(InputAction.CallbackContext ctx)
    {
        if (objetoSiendoArrastrado && ctx.performed)
        {
            if (!objetoCopiado.activeSelf)
            {
                destroyInstanceCopy();
                return;
            }
                
            // reemplazar cada array de materiales de la lista original por los de la lista de copias
            for (int i = 0; i < objetoCopiado.GetComponentsInChildren<Transform>().Length; i++)
            {
                Transform child = objetoCopiado.GetComponentsInChildren<Transform>()[i];
                child.gameObject.GetComponent<Renderer>().materials = materialesOriginalesObjeto[i];
            }

            objetoCopiado.GetComponent<BoxCollider>().enabled = true;
            objetoCopiado = null; // se "elimina" la referencia del objeto para que al hacer click derecho
                                  // no se vuelva a eliminar
            objetoSiendoArrastrado = false;

            if (!GameUIManager.Instance.activeObjectUI)
            {
                GameUIManager.Instance.crossHead.SetActive(false);
            }
            EventSystem.current.SetSelectedGameObject(null); // deseleccionar boton
        }
    }
    public void onRightClickPlacingObj(InputAction.CallbackContext ctx)
    {
        destroyInstanceCopy();
    }

    void destroyInstanceCopy()
    {
        objetoSiendoArrastrado = false;
        Destroy(objetoCopiado);
    }

    public void designMainObject(GameObject obj)
    {
        objeto = obj;
    }
}
