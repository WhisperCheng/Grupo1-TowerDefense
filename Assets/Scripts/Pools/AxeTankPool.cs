using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeTankPool : MonoBehaviour
{
    public GameObject axeTankPrefab; // Prefab del tanque con hacha
    public int poolSize = 10; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static AxeTankPool Instance;

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
            GameObject tank = Instantiate(axeTankPrefab);
            tank.SetActive(false);
            pool.Push(tank);
        }
    }

    public GameObject GetAxeTank()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de tanques con hacha vacía.");
            return null;
        }

        GameObject tank = pool.Pop();
        tank.SetActive(true);

        return tank;
    }

    public void ReturnAxeTank(GameObject returnedTank)
    {
        // Desactivar y devolver a la pool
        returnedTank.SetActive(false);
        pool.Push(returnedTank);
    }
}

