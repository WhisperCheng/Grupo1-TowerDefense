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
    public GameObject enemigoActual;
    public List<GameObject> objetivoActual = new List<GameObject>();

    //CONTROLADORES
    Animator animator;
    Rigidbody rb;
    NavMeshAgent navAgent;
    RaycastHit raycast;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        SeguirEnemigo();
        AtacarEnemigo();


        //Si no tiene enemigo seleccionado
        //Buscar enemigo dentro del rango
        //Si tiene enemigo lockeado
        //Ir hacia el
        //Si estas ya enfrente de el 
        //Atacarlo
            // si lo atacas y no esta enemigo lockeado null 

    }
    private void DetectarEnemigo()
    {
        objetivoActual.Clear();

        Collider[] listaChoques = Physics.OverlapSphere(transform.position, radio);

        foreach (Collider enemigo in listaChoques)
        {
            if (enemigo.CompareTag("Enemy"))
            {
                objetivoActual.Add(enemigo.gameObject);
            }
        }

        if (objetivoActual.Count > 0 && enemigoActual == null)
        {
            enemigoActual = objetivoActual[0];
        }

        if (enemigoActual != null && !objetivoActual.Contains(enemigoActual))
        {
            enemigoActual = null;
        }
    }
    void SeguirEnemigo()
    {
        Collider[] listaChoques;
        listaChoques = Physics.OverlapSphere(transform.position, radio);
        foreach (Collider enemigo in listaChoques)
        {
            if (enemigo.CompareTag("Enemy"))
            {
                animator.SetBool("Caminar", true);
                navAgent.SetDestination(enemigoMaza.position);
            }
        }
    }
    void AtacarEnemigo()
    {
        Vector3 origen = transform.position;
        Vector3 direccion = transform.forward;
        if (Physics.Raycast(origen, direccion, out raycast, longRayo))
        {
            radio = 0;
            animator.SetBool("Ataca", true);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radio);
        Gizmos.color = Color.blue;
    }
}
