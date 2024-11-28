using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemigoIA : MonoBehaviour
{

    protected NavMeshAgent agent;
    //Ataque
    [Header("Ataque")]
    public float attackDamage;

    [Header("Vida")]
    public float health;

    // Variables
    [Header("Variables Enemigo IA")]
    public float speed;
    public float actionRadio;
    public bool showActionRadio;
    public float cooldown;
    public string[] ignoreList;
    protected Vector3 destination;

    bool canAttack;
    int enemyMask = 1 << 6;

    public abstract void WhileWalking();

    protected Transform NearestForestHearthPos(Collider[] collisionsList)
    {
        Transform nearestForestHearthPos = null;
        float minorDistance = Mathf.Infinity;

        // Se comprueba y elige al corazón con menor distancia
        if (collisionsList.Length > 0)
        {
            foreach (Collider col in collisionsList)
            {
                // Distancia entre el enemigo y el objetivo
                float actualDistance = Vector3.Distance(transform.position, col.transform.position);
                if (actualDistance < minorDistance)
                {
                    /* Se detectan enemigos dentro del radio de acción pero hay que comprobar que
                     * no hay muros por delante*/
                    if (ThereAreNoObstacles(col.transform))
                    {
                        minorDistance = actualDistance;
                        nearestForestHearthPos = col.transform;
                    }
                }
            }
        }
        return nearestForestHearthPos; // puede llegar a ser nulo si no hay nada al rededor, hay que                    
    }                               // tenerlo en cuenta

    protected bool ThereAreNoObstacles(Transform closestObjetive)
    {
        bool enemyIsVisible = true;
        if (closestObjetive != null)
        {
            /* Primero intenté esta parte con raycastall pero al final solo funciono con un linecast, 
             * lo dejo como nota por si acaso*/

            RaycastHit hit;
            if (Physics.Linecast(transform.position, closestObjetive.position, out hit))
            {
                foreach (string name in ignoreList)
                {
                    if (hit.transform.tag != name)
                    {
                        enemyIsVisible = false;
                        break;
                    }
                }
                /*if (hit.transform.tag != "Proyectil" && hit.collider.gameObject.tag != "Enemigo"
                    && hit.transform.tag != "Aliado")
                {
                    enemyIsVisible = false;
                }*/
            }
        }

        return enemyIsVisible;
    }

}
