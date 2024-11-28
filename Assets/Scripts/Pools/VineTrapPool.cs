using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineTrapPool : MonoBehaviour
{
    public GameObject vineTrapPrefab; // Prefab de la trampa enredadera
    public int poolSize = 10; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static VineTrapPool Instance;

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
            GameObject trap = Instantiate(vineTrapPrefab);
            trap.SetActive(false);
            pool.Push(trap);
        }
    }

    public GameObject GetVineTrap()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de trampas enredaderas vacía.");
            return null;
        }

        GameObject trap = pool.Pop();
        trap.SetActive(true);

        return trap;
    }

    public void ReturnVineTrap(GameObject returnedTrap)
    {
        // Desactivar y devolver a la pool
        returnedTrap.SetActive(false);
        pool.Push(returnedTrap);
    }
}
