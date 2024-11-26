using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicImpactPool : MonoBehaviour
{
    public GameObject magicImpactPrefab;
    public int poolSize = 20;

    private Stack<GameObject> pool;
    public static MagicImpactPool Instance;

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
            GameObject impact = Instantiate(magicImpactPrefab);
            impact.SetActive(false);
            pool.Push(impact);
        }
    }

    public GameObject GetMagicImpact()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de impactos vacía.");
            return null;
        }

        GameObject impact = pool.Pop();
        impact.SetActive(true);

        return impact;
    }

    public void ReturnMagicImpact(GameObject returnedImpact)
    {
        // Desactivar y devolver a la pool
        returnedImpact.SetActive(false);
        pool.Push(returnedImpact);
    }
}

