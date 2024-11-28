using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyTowerPool : MonoBehaviour
{
    public GameObject allyTowerPrefab; // Prefab de la torre de aliados
    public int poolSize = 10; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static AllyTowerPool Instance;

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
            GameObject tower = Instantiate(allyTowerPrefab);
            tower.SetActive(false);
            pool.Push(tower);
        }
    }

    public GameObject GetAllyTower()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de torres de aliados vacía.");
            return null;
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
