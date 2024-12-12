using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceTankPool : MonoBehaviour
{
    public GameObject maceTankPrefab; // Prefab del tanque con maza
    public int poolSize = 10; // Tama�o de la pool

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool;
    public static MaceTankPool Instance;

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
        parent = new GameObject("MaceTank_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject tank = Instantiate(maceTankPrefab);
            tank.transform.parent = parent.transform;
            tank.SetActive(false);
            pool.Push(tank);
        }
    }

    public GameObject GetMaceTank()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de tanques con maza vac�a.");
            GameObject newTank = Instantiate(maceTankPrefab);
            newTank.transform.parent = parent.transform;
            return newTank;
        }

        GameObject tank = pool.Pop();
        tank.SetActive(true);

        return tank;
    }

    public void ReturnMaceTank(GameObject returnedTank)
    {
        // Desactivar y devolver a la pool
        returnedTank.SetActive(false);
        pool.Push(returnedTank);
    }
}

