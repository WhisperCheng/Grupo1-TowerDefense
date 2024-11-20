using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impacto : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] GameObject particulasImpacto;
    void OnCollisionEnter(Collision collision)
    {
        GameObject impacto = Instantiate(particulasImpacto);
        impacto.transform.position = transform.position;
        Destroy(gameObject); 
    }
}
