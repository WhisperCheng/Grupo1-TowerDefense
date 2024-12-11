using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyPool : MonoBehaviour
{
    public GameObject allyPrefab; // Prefab del aliado
    public int poolSize = 20; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static AllyPool Instance;

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
        for (int i = 0; i < poolSize; i++)
        {
            GameObject ally = Instantiate(allyPrefab);
            ally.transform.parent = parent.transform;
            ally.SetActive(false);
            pool.Push(ally);
        }
    }

    public GameObject GetAlly()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de aliados vacía.");
            return null;
        }

        GameObject ally = pool.Pop();
        ally.SetActive(true);

        return ally;
    }

    public void ReturnAlly(GameObject returnedAlly)
    {
        // Desactivar y devolver a la pool
        returnedAlly.SetActive(false);
        pool.Push(returnedAlly);
    }
}

