using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBuildingSpawner : MonoBehaviour
{
    private void OnEnable()
    {
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
