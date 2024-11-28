using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceTankPool : MonoBehaviour
{
    public GameObject maceTankPrefab; // Prefab del tanque con maza
    public int poolSize = 10; // Tamaño de la pool

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
            GameObject tank = Instantiate(maceTankPrefab);
            tank.SetActive(false);
            pool.Push(tank);
        }
    }

    public GameObject GetMaceTank()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de tanques con maza vacía.");
            return null;
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

