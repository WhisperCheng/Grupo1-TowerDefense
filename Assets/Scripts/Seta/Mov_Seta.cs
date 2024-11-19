using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mov_Seta : MonoBehaviour
{
    //VARIABLES
    int radio = 5;
    float vida = 10;
    float fuerza = 20;

    //CONTROLADORES
    Animator animator;
    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject)
        {
            
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radio);
        Gizmos.color = Color.red;
    }
}
