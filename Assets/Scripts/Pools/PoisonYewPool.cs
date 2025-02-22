using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonYewPool : MonoBehaviour
{
    public GameObject poisonYewPrefab; // Prefab del tejo venenoso
    public int poolSize = 20; // Tama�o de la pool

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool;
    public static PoisonYewPool Instance;

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
        parent = new GameObject("PoisonYew_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject yew = Instantiate(poisonYewPrefab);
            yew.transform.parent = parent.transform;
            yew.SetActive(false);
            pool.Push(yew);
        }
    }

    public GameObject GetPoisonYew()
    {
        if (pool.Count == 0)
        {
            GameObject newYew = Instantiate(poisonYewPrefab);
            newYew.transform.parent = parent.transform;
            newYew.SetActive(true);
            return newYew;
        }

        GameObject yew = pool.Pop();
        IPoolable poolablePlant = yew.GetComponent<IPoolable>();
        yew.SetActive(true);
        // Solo se resetea si ya ha existido previamente
        yew = poolablePlant.RestoreToDefault();

        return yew;
    }

    public void ReturnPoisonYew(GameObject returnedYew)
    {
        // Desactivar y devolver a la pool
        returnedYew.SetActive(false);
        pool.Push(returnedYew);
    }
}