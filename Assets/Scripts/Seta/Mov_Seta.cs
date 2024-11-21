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

    //GAMEOBJECT
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
        transform.LookAt(enemigoMaza.transform.position);

        foreach (Collider enemigo in listaChoques)
        {
            if (enemigo.CompareTag("Enemy"))
            {
                animator.SetBool("Caminar", true);
                NavAgent.SetDestination(enemigoMaza.position);
            }
            else
            {
                animator.SetBool("Caminar", false);
            }
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            animator.SetBool("Atacar", true);
        }
        else
        {
            animator.SetBool("Atacar", false);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radio);
        Gizmos.color = Color.red;
    }
}
