using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapPool : MonoBehaviour
{
    public GameObject spikeTrapPrefab; // Prefab de la trampa de pinchos
    public int poolSize = 10; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static SpikeTrapPool Instance;

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
            GameObject trap = Instantiate(spikeTrapPrefab);
            trap.SetActive(false);
            pool.Push(trap);
        }
    }

    public GameObject GetSpikeTrap()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de trampas de pinchos vacía.");
            return null;
        }

        GameObject trap = pool.Pop();
        trap.SetActive(true);

        return trap;
    }

    public void ReturnSpikeTrap(GameObject returnedTrap)
    {
        // Desactivar y devolver a la pool
        returnedTrap.SetActive(false);
        pool.Push(returnedTrap);
    }
}

