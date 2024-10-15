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
    Material[] materialesObjeto;
    Material[] materialesOriginalesObjeto;
    //Color32 colorOriginalObjeto;
    private bool objetoSiendoArrastrado = false;
    public PlayerInput playerInput;

    public Button button1;

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
            objetoCopiado.GetComponent<PlaceableObject>().setIsACopy(true);
            // TODO: Sin uso, pero se queda así por si hace falta luego eliminar todas las copias

            // Esto es para que al colocarlo no se buguee con el raycast todo el rato, hasta que se termine de colocar
            objetoCopiado.GetComponent<BoxCollider>().enabled = false;
            seleccionarObjeto();
            objetoSiendoArrastrado = true;
        }
    }

    void seleccionarObjeto()
    {
        materialesObjeto = objetoCopiado.GetComponent<Renderer>().materials;
        materialesOriginalesObjeto = new Material[materialesObjeto.Length];
        for (int i = 0; i < materialesObjeto.Length; i++)
        {
            Material mat = materialesObjeto[i];
            materialesOriginalesObjeto[i] = new Material(mat);
            // Importante poner new y crear un nuevo material, de lo
            // contrario el material seguirá vinculado al anterior
            mat.color = selectedColor;
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

    public void onClickButton(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            button1.onClick.Invoke();
        }
    }

    public void onClickPlaceObj(InputAction.CallbackContext ctx)
    {
        if (objetoSiendoArrastrado && ctx.performed)
        {
            //bool btn1Click = playerInput.actions["Click"].ReadValue<float>() > 0 ? true : false;
            //if (btn1Click)
            //{
            //playerInput.actions["Click"].performed += c => Debug.Log("s");
            objetoCopiado.GetComponent<Renderer>().materials = materialesOriginalesObjeto;
            objetoCopiado.GetComponent<BoxCollider>().enabled = true;
            //NumObjetos.numObjetos++;
            //NumObjetos.actualizarNumObjetos();
            objetoSiendoArrastrado = false;
            //}
        }
    }

    public void designMainObject(GameObject obj)
    {
        objeto = obj;
    }
}
