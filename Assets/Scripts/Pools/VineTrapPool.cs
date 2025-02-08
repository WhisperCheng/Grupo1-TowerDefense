using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineTrapPool : MonoBehaviour
{
    public GameObject vineTrapPrefab; // Prefab de la trampa enredadera
    public int poolSize = 10; // Tamaño de la pool

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

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
        grandParent = GameObject.Find(grandParentName);
    }

    void Start()
    {
        SetupPool();
    }

    void SetupPool()
    {
        pool = new Stack<GameObject>();
        parent = new GameObject("VineTrap_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject trap = Instantiate(vineTrapPrefab);
            trap.transform.parent = parent.transform;
            trap.SetActive(false);
            pool.Push(trap);
        }
    }

    public GameObject GetVineTrap()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de trampas enredaderas vacía.");
            GameObject newTrap = Instantiate(vineTrapPrefab);
            newTrap.transform.parent = parent.transform;
            return newTrap;
        }

        GameObject trap = pool.Pop();
        IPoolable poolableTrap = trap.GetComponent<IPoolable>();
        poolableTrap.RestoreToDefault();
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
