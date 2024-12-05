using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Herramientas y métodos helper para la detección de enemigos y aliados
/// </summary>
public static class EntityUtils
{

    /// --- Funciones y "Herramientas" para las Clases de Torres, Aliados Y Enemigos --- ///
    public static Transform GetNearestForestHearthPos(Vector3 actualPos, string[] ignoreTagList)
    {
        Transform nearestForestHearthPos = null;

        GameObject[] heartList = GameObject.FindGameObjectsWithTag(GameManager.Instance.tagCorazonDelBosque);

        // Se comprueba y elige al corazón con menor distancia
        if (heartList.Length > 0)
        {
            nearestForestHearthPos = GetNearestTransformInList(heartList, actualPos, ignoreTagList);
        }
        return nearestForestHearthPos; // puede llegar a ser nulo si no hay nada al rededor, hay que                    
    }                               // tenerlo en cuenta

    private static Transform GetNearestTransformInList(GameObject[] gameObjectList, Vector3 actualPos, string[] ignoreTagList)
    {
        Transform nearestGameObjectPos = null;
        float minorDistance = Mathf.Infinity;

        foreach (GameObject gameObj in gameObjectList)
        {
            // Distancia entre el enemigo y el objetivo
            float actualDistance = Vector3.Distance(actualPos, gameObj.transform.position);
            if (actualDistance < minorDistance)
            {
                /* Se detectan enemigos dentro del radio de acción pero hay que comprobar que
                 * no hay muros por delante*/
                if (ThereAreNoObstacles(gameObj.transform, actualPos, ignoreTagList))
                {
                    minorDistance = actualDistance;
                    nearestGameObjectPos = gameObj.transform;
                }
            }
        }

        return nearestGameObjectPos;
    }

    private static Transform GetNearestCollisionInList(Collider[] colliderList, Vector3 actualPos, string[] ignoreTagList,
        bool ignoreVisibility)
    {
        Transform nearestObjetivePos = null;
        float minorDistance = Mathf.Infinity;

        foreach (Collider col in colliderList)
        {
            // Distancia entre el enemigo y el objetivo
            float actualDistance = Vector3.Distance(actualPos, col.transform.position);
            if (actualDistance < minorDistance)
            {
                if (ignoreVisibility)
                { /* Si se ignora la visibilidad directa del enemigo, se escoge siempre el más cercano aunque
                   hayan obstáculos por delante */
                    minorDistance = actualDistance;
                    nearestObjetivePos = col.transform;
                }
                if (!ignoreVisibility && ThereAreNoObstacles(col.transform, actualPos, ignoreTagList))
                {
                    /* Si no se ignora la visibilidad directa, se detectan enemigos dentro del radio de acción
                     * comprobando que no hay muros por delante*/
                    minorDistance = actualDistance;
                    nearestObjetivePos = col.transform;
                }
            }
        }

        return nearestObjetivePos;
    }
    private static Transform GetNearestCollisionInList(Collider[] colliderList, Vector3 actualPos)
    {
        return GetNearestCollisionInList(colliderList, actualPos, null, true);
    }


    private static bool ThereAreNoObstacles(Transform nearestObjetive, Vector3 pos, string[] ignoreTagList)
    {
        bool noObstacles = true;
        if (nearestObjetive != null)
        {
            RaycastHit hit;
            if (Physics.Linecast(pos, nearestObjetive.position, out hit))
            {/*
                if (hit.transform.tag != "Proyectil" && hit.collider.gameObject.tag != "Enemigo"
                    && hit.transform.tag != "Gnomo")
                {
                    noObstacles = false;
                }*/
                if (ignoreTagList != null)
                {
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
        }

        return noObstacles;
    }

    private static bool ThereAreNoObstacles(Transform nearestObjetive, Vector3 pos)
    {
        return ThereAreNoObstacles(nearestObjetive, pos, null);
    }

    public static Transform NearestRival(Collider[] colliderList, Vector3 pos, string[] ignoreTagList, bool ignoreDirectVisbility)
    {
        // Esta lista almacenará el resultado de llamar a OverlapSphere
        Transform nearestRival = null;

        // Se comprueba y elige el enemigo con menor distancia
        if (colliderList.Length > 0)
        {
            nearestRival = GetNearestCollisionInList(colliderList, pos, ignoreTagList, ignoreDirectVisbility);
        }
        return nearestRival; // puede llegar a ser nulo si no hay nada al rededor, hay que                    
        // tenerlo en cuenta
    }

}
