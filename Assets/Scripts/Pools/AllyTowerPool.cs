using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyTowerPool : MonoBehaviour
{
    public GameObject allyTowerPrefab; // Prefab de la torre de aliados
    public int poolSize = 10; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static AllyTowerPool Instance;

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

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
        parent = new GameObject("AllyTower_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject tower = Instantiate(allyTowerPrefab);
            tower.transform.parent = parent.transform;
            tower.SetActive(false);
            pool.Push(tower);
        }
    }

    public GameObject GetAllyTower()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de torres de aliados vacía.");
            GameObject newTower = Instantiate(allyTowerPrefab);
            newTower.transform.parent = parent.transform;
            newTower.SetActive(true);
            return newTower;
        }

        GameObject tower = pool.Pop();
        tower.SetActive(true);

        return tower;
    }

    public void ReturnAllyTower(GameObject returnedTower)
    {
        // Desactivar y devolver a la pool
        returnedTower.SetActive(false);
        pool.Push(returnedTower);
    }
}
