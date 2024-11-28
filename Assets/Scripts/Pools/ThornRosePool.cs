using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornRosePool : MonoBehaviour
{
    public GameObject thornRosePrefab; // Prefab de la rosa lanzaespinas
    public int poolSize = 20; // Tamaño de la pool

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
            GameObject thornRose = Instantiate(thornRosePrefab);
            thornRose.SetActive(false);
            pool.Push(thornRose);
        }
    }

    public GameObject GetThornRose()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de rosas lanzaespinas vacía.");
            return null;
        }

        GameObject thornRose = pool.Pop();
        thornRose.SetActive(true);

        return thornRose;
    }

    public void ReturnThornRose(GameObject returnedThornRose)
    {
        // Desactivar y devolver a la pool
        returnedThornRose.SetActive(false);
        pool.Push(returnedThornRose);
    }
}

