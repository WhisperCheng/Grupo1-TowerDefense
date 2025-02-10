using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeTankPool : MonoBehaviour
{
    public GameObject axeTankPrefab; // Prefab del tanque con hacha
    public int poolSize = 10; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static AxeTankPool Instance;

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
        parent = new GameObject("AxeTank_PC");
        for (int i = 0; i < poolSize; i++)
        {
            GameObject tank = Instantiate(axeTankPrefab);
            tank.transform.parent = parent.transform;
            tank.SetActive(false);
            pool.Push(tank);
        }
    }

    public GameObject GetAxeTank()
    {
        if (pool.Count == 0)
        {
            GameObject newTank = Instantiate(axeTankPrefab);
            newTank.transform.parent = parent.transform;
            return newTank;
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

