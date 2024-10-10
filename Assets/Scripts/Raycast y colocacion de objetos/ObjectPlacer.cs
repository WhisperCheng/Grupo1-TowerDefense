using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public GameObject objeto;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void asignMainObject(GameObject obj)
    {
        PlaceManager.Instance.designMainObject(obj);
        PlaceManager.Instance.generarObjeto(obj);

    }
}
