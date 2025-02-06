using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBuildingSpawner : MonoBehaviour
{
    public RecolectorTorre recolector;
    private void OnEnable()
    {
        recolector.AddGemsEvent();
        SpawnResourceTower();
    }

    void SpawnResourceTower()
    {
        if (ResourceTowerPool.Instance != null)
        {
            GameObject tower = ResourceTowerPool.Instance.GetResourceTower();
            if (tower != null)
            {
                tower.transform.position = transform.position;
                tower.transform.rotation = transform.rotation;
            }
        }
        else
        {
            Debug.LogError("ResourceTowerPool.Instance no está inicializado.");
        }
    }
}
