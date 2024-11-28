using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherPool : MonoBehaviour
{
    public GameObject archerPrefab; // Prefab del arquero
    public int poolSize = 15; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static ArcherPool Instance;

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
            GameObject archer = Instantiate(archerPrefab);
            archer.SetActive(false);
            pool.Push(archer);
        }
    }

    public GameObject GetArcher()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de arqueros vacía.");
            return null;
        }

        GameObject archer = pool.Pop();
        archer.SetActive(true);

        return archer;
    }

    public void ReturnArcher(GameObject returnedArcher)
    {
        // Desactivar y devolver a la pool
        returnedArcher.SetActive(false);
        pool.Push(returnedArcher);
    }
}

