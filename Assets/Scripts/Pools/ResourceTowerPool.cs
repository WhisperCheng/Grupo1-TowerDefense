using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTowerPool : MonoBehaviour
{
    public GameObject resourceTowerPrefab; // Prefab de la torre de recursos
    public int poolSize = 10; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static ResourceTowerPool Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        SetupPool();
    }

    void SetupPool()
    {
        pool = new Stack<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject tower = Instantiate(resourceTowerPrefab);
            tower.SetActive(false);
            pool.Push(tower);
        }
    }

    public GameObject GetResourceTower()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de torres de recursos vacía.");
            return null;
        }

        GameObject tower = pool.Pop();
        tower.SetActive(true);

        return tower;
    }

    public void ReturnResourceTower(GameObject returnedTower)
    {
        // Desactivar y devolver a la pool
        returnedTower.SetActive(false);
        pool.Push(returnedTower);
    }
}

