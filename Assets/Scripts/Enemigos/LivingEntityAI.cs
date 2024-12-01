using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public abstract class LivingEntityAI : MonoBehaviour
{
    protected NavMeshAgent agent;
    [Header("Tags de obst�culos a ignorar")]
    public string[] ignoreTagList;

    public virtual bool ThereAreNoObstacles(Transform nearestObjetive) {
        bool noObstacles = true;
        if (nearestObjetive != null)
        {
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

    public abstract void Init();
}
