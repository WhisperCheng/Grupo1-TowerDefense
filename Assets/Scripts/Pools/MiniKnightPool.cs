using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniKnightPool : MonoBehaviour
{
    public GameObject miniKnightPrefab; // Prefab del mini caballero
    public int poolSize = 15; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static MiniKnightPool Instance;

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
            GameObject knight = Instantiate(miniKnightPrefab);
            knight.SetActive(false);
            pool.Push(knight);
        }
    }

    public GameObject GetMiniKnight()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de mini caballeros vacía.");
            return null;
        }

        GameObject knight = pool.Pop();
        knight.SetActive(true);

        return knight;
    }

    public void ReturnMiniKnight(GameObject returnedKnight)
    {
        // Desactivar y devolver a la pool
        returnedKnight.SetActive(false);
        pool.Push(returnedKnight);
    }
}

