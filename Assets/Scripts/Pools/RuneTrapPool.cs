using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneTrapPool : MonoBehaviour
{
    public GameObject runeTrapPrefab; // Prefab de la trampa de runa
    public int poolSize = 10; // Tamaño de la pool

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

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
        grandParent = GameObject.Find(grandParentName);
    }

    void Start()
    {
        SetupPool();
    }

    void SetupPool()
    {
        pool = new Stack<GameObject>();
        parent = new GameObject("RuneTrap_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject trap = Instantiate(runeTrapPrefab);
            trap.transform.parent = parent.transform;
            trap.SetActive(false);
            pool.Push(trap);
        }
    }

    public GameObject GetRuneTrap()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de trampas de runa vacía.");
            GameObject newTrap = Instantiate(runeTrapPrefab);
            newTrap.transform.parent = parent.transform;
            return newTrap;
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

