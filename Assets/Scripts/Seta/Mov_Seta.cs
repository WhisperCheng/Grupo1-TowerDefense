using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Mov_Seta : MonoBehaviour
{
    //VARIABLES
    int radio = 5;
    float vida = 10;
    float fuerza = 20;
    public Transform enemigoMaza; //maza o el que sea que sigue la seta

    //CONTROLADORES
    Animator animator;
    Rigidbody rb;
    NavMeshAgent NavAgent;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        NavAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        SeguirEnemigo();
    }
    void SeguirEnemigo()
    {
        Collider[] listaChoques;
        listaChoques = Physics.OverlapSphere(transform.position, radio);
        foreach (Collider enemigo in listaChoques)
        {
            Debug.Log(enemigo);
            if (enemigo.CompareTag("Enemy"))
            {
                NavAgent.SetDestination(enemigoMaza.position);
            }
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject)
        {
            // cuando colisiones hacer animacion de atacar 
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radio);
        Gizmos.color = Color.red;
    }
}
