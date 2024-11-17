using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impacto : MonoBehaviour
{
    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject); 
    }
}
