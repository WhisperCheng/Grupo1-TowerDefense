using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Mov_Seta : MonoBehaviour
{
    //VARIABLES
    int radio = 15;
    int longRayo = 1;
    float vida = 10;
    float fuerza = 20;

    //GAMEOBJECT
    public Transform enemigoMaza; //maza o el que sea que sigue la seta
    public Transform baseAliada;

    //CONTROLADORES
    Animator animator;
    Rigidbody rb;
    NavMeshAgent NavAgent;
    RaycastHit raycast;

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

        Vector3 origen = transform.position;
        Vector3 direccion = transform.forward;
        foreach (Collider enemigo in listaChoques)
        {
            if (enemigo.CompareTag("Enemy"))
            {
                animator.SetBool("Caminar", true);
                NavAgent.SetDestination(enemigoMaza.position);
            }
            else
            {
                radio = 15;
                animator.SetBool("Caminar", false);
                //NavAgent.SetDestination(baseAliada.position);
            }
        }
        if (Physics.Raycast(origen, direccion, out raycast, longRayo))
        {
            radio = 0;
            animator.SetBool("Ataca", true);
        }
        /*else
        {
            radio = 15;
            animator.SetBool("Ataca", false);
        }*/
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radio);
        Gizmos.color = Color.blue;
    }
}
