using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTowerPool : MonoBehaviour
{
    public GameObject resourceTowerPrefab; // Prefab de la torre de recursos
    public int poolSize = 10; // Tamaño de la pool

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

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
        grandParent = GameObject.Find(grandParentName);
    }

    void Start()
    {
        SetupPool();
    }

    void SetupPool()
    {
        pool = new Stack<GameObject>();
        parent = new GameObject("ResourceTower_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject tower = Instantiate(resourceTowerPrefab);
            tower.transform.parent = parent.transform;
            tower.SetActive(false);
            pool.Push(tower);
        }
    }

    public GameObject GetResourceTower()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de torres de recursos vacía.");
            GameObject newTower = Instantiate(resourceTowerPrefab);
            newTower.transform.parent = parent.transform;
            return newTower;
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

