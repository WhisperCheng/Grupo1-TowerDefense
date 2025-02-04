using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicArcherPool : MonoBehaviour
{
    public GameObject archerPrefab; // Prefab del mini caballero
    public int poolSize = 15; // Tamaño de la pool

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool;
    public static MagicArcherPool Instance;

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
        parent = new GameObject("MagicArcher_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject archer = Instantiate(archerPrefab);
            archer.transform.parent = parent.transform;
            archer.SetActive(false);
            pool.Push(archer);
        }
    }

    public GameObject GetMagicArcher()
    {
        if (pool.Count == 0)
        {
            GameObject newArcher = Instantiate(archerPrefab);
            newArcher.transform.parent = parent.transform;
            newArcher.SetActive(true);

            return newArcher;
        }

        GameObject archer = pool.Pop();
        IPoolable poolableArcher = archer.GetComponent<IPoolable>();

        archer.SetActive(true);

        // Solo se resetea si ya ha existido previamente
        archer = poolableArcher.RestoreToDefault();

        return archer;
    }

    public void ReturnMagicArcher(GameObject returnedArcher)
    {
        // Desactivar y devolver a la pool
        returnedArcher.SetActive(false);

        // Agregar de vuelta a la pool
        pool.Push(returnedArcher);
    }
}
