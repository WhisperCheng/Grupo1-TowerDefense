using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Herramientas y métodos helper para la detección de enemigos, aliados y waypoints
/// </summary>
public static class EntityUtils
{

    /// --- Funciones y "Herramientas" para las Clases de Torres, Aliados Y Enemigos --- ///
    public static Transform GetNearestForestHearthPos(Vector3 actualPos, string[] ignoreTagList)
    {
        Transform nearestForestHearth = null;

        GameObject[] heartList = GameObject.FindGameObjectsWithTag(GameManager.Instance.tagCorazonDelBosque);

        // Se comprueba y elige al corazón con menor distancia
        if (heartList.Length > 0)
        {
            nearestForestHearth = GetNearestTransform(heartList, actualPos, ignoreTagList);
        }
        return nearestForestHearth; // puede llegar a ser nulo si no hay nada al rededor, hay que                    
    }                               // tenerlo en cuenta

    public static Transform GetNearestWayPoint(Vector3 actualPos)
    {
        Transform nearestWaypoint = null;

        GameObject[] waypointList = GameManager.Instance.wayPoints;

        // Se comprueba y elige al waypoint con menor distancia
        if (waypointList.Length > 0)
        {
            nearestWaypoint = GetNearestWaypointTransform(waypointList, actualPos);
        }
        return nearestWaypoint; // puede llegar a ser nulo si no hay nada al rededor, hay que                    
    }

    private static Transform GetNearestTransform(GameObject[] gameObjectList, Vector3 actualPos, string[] ignoreTagList)
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

    private static Transform GetNearestWaypointTransform(GameObject[] gameObjectList, Vector3 actualPos)
    {
        Transform nearestGameObjectPos = null;
        float minorDistance = Mathf.Infinity;

        foreach (GameObject gameObj in gameObjectList)
        {
            // Distancia entre el enemigo y el objetivo
            float actualDistance = Vector3.Distance(actualPos, gameObj.transform.position);
            if (actualDistance < minorDistance)
            {
                minorDistance = actualDistance;
                nearestGameObjectPos = gameObj.transform;
            }
        }

        return nearestGameObjectPos;
    }

    private static Transform GetNearestRivalCollision(Collider[] colliderList, Vector3 actualPos, string[] ignoreTagList,
        bool ignoreVisibility)
    {
        Transform nearestObjetivePos = null;
        float minorDistance = Mathf.Infinity;

        foreach (Collider col in colliderList)
        {
            IDamageable entity = col.GetComponent<IDamageable>();
            // Distancia entre el enemigo y el objetivo
            float actualDistance = Vector3.Distance(actualPos, col.transform.position);
            if (entity != null && entity.GetHealth() > 0 && actualDistance < minorDistance)
            {   // Si se detecta un rival vivo, y la distance actual es menor que las anteriores, se procede

                // En los siguientes casos se tendrá en cuenta si el enemigo tiene vida superior a 0 (si está vivo)
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
    
    private static Transform GetNearestRivalCollisionOnNavMesh(NavMeshAgent agent, Collider[] colliderList, Vector3 actualPos, 
        string[] ignoreTagList, bool ignoreVisibility, float reachDistance)
    {
        Transform nearestObjetivePos = null;
        float minorDistance = Mathf.Infinity;

        foreach (Collider col in colliderList)
        {
            IDamageable entity = col.GetComponent<IDamageable>();
            // Distancia entre el enemigo y el objetivo
            float actualDistance = Vector3.Distance(actualPos, col.transform.position);

            NavMeshHit hit = new NavMeshHit();
            bool canReachRivalOnNavMesh = NavMesh.SamplePosition(col.transform.position, out hit, reachDistance,
                new NavMeshQueryFilter()
                {
                    agentTypeID = agent.agentTypeID,
                    areaMask = agent.areaMask
                });
            // Si se trata de una entidad con vida > 0 y la distancia es una menor a las anteriores y puede alcanzarla
            // dentro del navmesh, se prosigue
            if (entity != null && entity.GetHealth() > 0 && actualDistance < minorDistance && canReachRivalOnNavMesh)
            {   // Si se detecta un rival vivo, y la distance actual es menor que las anteriores, se procede

                // En los siguientes casos se tendrá en cuenta si el enemigo tiene vida superior a 0 (si está vivo)
                if (ignoreVisibility)
                { /* Si se ignora la visibilidad directa del enemigo, se escoge siempre el más cercano aunque
                   hayan obstáculos por delante */
                    minorDistance = actualDistance;
                    nearestObjetivePos = col.transform;
                    Debug.DrawLine(actualPos, col.transform.position, Color.green);
                }
                if (!ignoreVisibility && ThereAreNoObstacles(col.transform, actualPos, ignoreTagList))
                {
                    /* Si no se ignora la visibilidad directa, se detectan enemigos dentro del radio de acción
                     * comprobando que no hay muros por delante*/
                    minorDistance = actualDistance;
                    nearestObjetivePos = col.transform;
                    Debug.DrawLine(actualPos, col.transform.position, Color.green);
                }
            }
        }

        return nearestObjetivePos;
    }
    private static Transform GetNearestCollision(Collider[] colliderList, Vector3 actualPos)
    {
        return GetNearestRivalCollision(colliderList, actualPos, null, true);
    }


    private static bool ThereAreNoObstacles(Transform nearestObjetive, Vector3 pos, string[] ignoreTagList)
    {
        bool noObstacles = true;
        if (nearestObjetive != null)
        {
            RaycastHit hit;
            if (Physics.Linecast(pos, nearestObjetive.position, out hit))
            {
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
            nearestRival = GetNearestRivalCollision(colliderList, pos, ignoreTagList, ignoreDirectVisbility);
        }
        return nearestRival; // puede llegar a ser nulo si no hay nada al rededor, hay que                    
        // tenerlo en cuenta
    }
    public static Transform NearestRivalOnNavMesh(NavMeshAgent agent, Collider[] colliderList, Vector3 pos, string[] ignoreTagList, 
        bool ignoreDirectVisbility, float reachDistanceOutSideNavMesh)
    {
        // Esta lista almacenará el resultado de llamar a OverlapSphere
        Transform nearestRival = null;

        // Se comprueba y elige el enemigo con menor distancia
        if (colliderList.Length > 0)
        {
            nearestRival = GetNearestRivalCollisionOnNavMesh(agent, colliderList, pos, ignoreTagList, ignoreDirectVisbility, 
                reachDistanceOutSideNavMesh);
        }
        return nearestRival; // puede llegar a ser nulo si no hay nada al rededor, hay que tenerlo en cuenta
    }
}
