using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSlashPool : MonoBehaviour
{
    public GameObject poisonSlashPrefab; // Prefab del tajo venenoso
    public int poolSize = 20; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static PoisonSlashPool Instance;

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
            GameObject slash = Instantiate(poisonSlashPrefab);
            slash.SetActive(false);
            pool.Push(slash);
        }
    }

    public GameObject GetPoisonSlash()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de tajos venenosos vacía.");
            return null;
        }

        GameObject slash = pool.Pop();
        slash.SetActive(true);

        return slash;
    }

    public void ReturnPoisonSlash(GameObject returnedSlash)
    {
        // Desactivar y devolver a la pool
        returnedSlash.SetActive(false);
        pool.Push(returnedSlash);
    }
}

