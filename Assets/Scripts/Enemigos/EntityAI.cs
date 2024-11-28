using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EntityAI : MonoBehaviour
{
    protected NavMeshAgent agent;
    [Header("Vida")]
    public float health;
    [Header("Tags de obstáculos a ignorar")]
    public string[] ignoreTagList;

    public virtual bool ThereAreNoObstacles(Transform nearestObjetive) {
        bool noObstacles = true;
        if (nearestObjetive != null)
        {
            /* Primero intenté esta parte con raycastall pero al final solo funciono con un linecast, 
             * lo dejo como nota por si acaso*/

            RaycastHit hit;
            if (Physics.Linecast(transform.position, nearestObjetive.position, out hit))
            {/*
                if (hit.transform.tag != "Proyectil" && hit.collider.gameObject.tag != "Enemigo"
                    && hit.transform.tag != "Gnomo")
                {
                    noObstacles = false;
                }*/
                foreach (string name in ignoreTagList)
                {
                    if (hit.transform.tag != name)
                    {
                        noObstacles = false;
                        break;
                    }
                }
            }
        }

        return noObstacles;
    }
}
