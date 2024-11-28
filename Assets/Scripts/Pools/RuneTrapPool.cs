using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneTrapPool : MonoBehaviour
{
    public GameObject runeTrapPrefab; // Prefab de la trampa de runa
    public int poolSize = 10; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static RuneTrapPool Instance;

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
            GameObject trap = Instantiate(runeTrapPrefab);
            trap.SetActive(false);
            pool.Push(trap);
        }
    }

    public GameObject GetRuneTrap()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de trampas de runa vacía.");
            return null;
        }

        GameObject trap = pool.Pop();
        trap.SetActive(true);

        return trap;
    }

    public void ReturnRuneTrap(GameObject returnedTrap)
    {
        // Desactivar y devolver a la pool
        returnedTrap.SetActive(false);
        pool.Push(returnedTrap);
    }
}

