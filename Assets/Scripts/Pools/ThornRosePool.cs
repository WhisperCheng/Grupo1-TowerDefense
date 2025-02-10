using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornRosePool : MonoBehaviour
{
    public GameObject thornRosePrefab; // Prefab de la rosa lanzaespinas
    public int poolSize = 20; // Tamaño de la pool

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool;
    public static ThornRosePool Instance;

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
        parent = new GameObject("ThornRose_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject thornRose = Instantiate(thornRosePrefab);
            thornRose.transform.parent = parent.transform;
            thornRose.SetActive(false);
            pool.Push(thornRose);
        }
    }

    public GameObject GetThornRose()
    {
        if (pool.Count == 0)
        {
            GameObject newThornRose = Instantiate(thornRosePrefab);
            newThornRose.transform.parent = parent.transform;
            newThornRose.SetActive(true);
            return newThornRose;
        }

        GameObject thornRose = pool.Pop();
        IPoolable poolablePlant = thornRose.GetComponent<IPoolable>();
        thornRose.SetActive(true);
        // Solo se resetea si ya ha existido previamente
        thornRose = poolablePlant.RestoreToDefault();

        return thornRose;
    }

    public void ReturnThornRose(GameObject returnedThornRose)
    {
        // Desactivar y devolver a la pool
        returnedThornRose.SetActive(false);
        pool.Push(returnedThornRose);
    }
}

