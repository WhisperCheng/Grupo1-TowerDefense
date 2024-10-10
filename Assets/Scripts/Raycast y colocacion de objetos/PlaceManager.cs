using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceManager : MonoBehaviour

{
    public static PlaceManager Instance { get; private set; }

    public Color selectedColor;
    public Color originalColor;

    private static GameObject objeto;
    private static GameObject objetoCopiado;
    Material[] materialesObjeto;
    Material[] materialesOriginalesObjeto;
    //Color32 colorOriginalObjeto;
    private bool objetoSiendoArrastrado = false;

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
            Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit golpeRayo;
            bool colisionConRayo = Physics.Raycast(rayo, out golpeRayo, 100000f);
            objetoCopiado = Instantiate(objeto,
                    !colisionConRayo ? objeto.transform.position : golpeRayo.point, objeto.transform.rotation);
            // Se cambia el "tag" <<original>> del objeto a falso para posteriormente poder borrar todos
            // excepto el original
            objetoCopiado.GetComponent<PlaceableObject>().setIsACopy(true);
            // TODO: Sin uso, pero se queda así por si hace falta luego eliminar todas las copias

            // Esto es para que al colocarlo no se buguee con el raycast todo el rato, hasta que se termine de colocar
            //boxColliderSizeCopia = objetoCopiado.GetComponent<BoxCollider>().size;
            objetoCopiado.GetComponent<BoxCollider>().enabled = false;
            //objetoCopiado.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(49f, 255f, 255f)); // x

            seleccionarObjeto();


            objetoSiendoArrastrado = true;
            //}
        }
    }

    void seleccionarObjeto()
    {
        materialesObjeto = objetoCopiado.GetComponent<Renderer>().materials;
        //Debug.Log(materialesOriginalesObjeto);
        materialesOriginalesObjeto = materialesObjeto;
        foreach (Material mat in materialesObjeto)
        {
            //mat.color = new Color32(49, 255, 255, 255);
            //mat.color = new Color(49, mat.color.g, mat.color.b, 0.5f);
            originalColor = mat.color;
            mat.color = selectedColor;
            //mat.SetColor("_Color", newColor);
            Debug.Log(mat.color);
        }

        /*colorOriginalObjeto = objetoCopiado.GetComponent<Renderer>().material.color;
        objetoCopiado.GetComponent<Renderer>().material.color = new Color32(49, 255, 255, 255); // v
        */
    }



    private void manageObjectPlacement()
    {
        if (objetoSiendoArrastrado)
        {
            Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit golpeRayo;
            if (Physics.Raycast(rayo, out golpeRayo, 100000f))
            {
                objetoCopiado.gameObject.transform.position = golpeRayo.point;
            }
            //Debug.Log(objetoCopiado.gameObject.transform.position);
            if (Input.GetMouseButtonDown(0)/* && EventSystem.current.IsPointerOverGameObject()*/)
            {
                //objetoCopiado.GetComponent<Renderer>().SetMaterials(new List<Material>(materialesOriginalesObjeto)); // no funciona
                //var mats = objetoCopiado.GetComponent<Renderer>().materials;
                for (int i = 0; i < materialesOriginalesObjeto.Length; i++)
                {
                    objetoCopiado.GetComponent<Renderer>().materials[i] = materialesOriginalesObjeto[i];
                    //https://www.youtube.com/watch?v=355zyQRiQP4
                }
                objetoCopiado.GetComponent<BoxCollider>().enabled = true;
                //NumObjetos.numObjetos++;
                //NumObjetos.actualizarNumObjetos();
                objetoSiendoArrastrado = false;
                //objetoCopiado = null;
            }
        }
    }

    public void designMainObject(GameObject obj)
    {
        objeto = obj; 
    }
}
