using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilMagicImpactPool : MonoBehaviour
{
    public GameObject magicImpactPrefab;
    public int poolSize = 20;

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool;
    public static EvilMagicImpactPool Instance;

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
        if (grandParent == null)
        {
            Debug.LogError($"No se encontr� un objeto llamado '{grandParentName}' en la escena.");
        }
    }

    void Start()
    {
        SetupPool();
    }

    void SetupPool()
    {

        pool = new Stack<GameObject>();
        parent = new GameObject("EvilMagicImpact_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject impact = Instantiate(magicImpactPrefab);
            impact.transform.parent = parent.transform;
            impact.SetActive(false);
            pool.Push(impact);
        }
    }

    public GameObject GetEvilMagicImpact()
    {
        if (pool.Count == 0)
        {
            GameObject newImpact = Instantiate(magicImpactPrefab);
            newImpact.transform.parent = parent.transform; 
            return newImpact;
        }

        GameObject impact = pool.Pop();
        impact.SetActive(true);
        return impact;
    }

    public void ReturnEvilMagicImpact(GameObject returnedImpact)
    {
        // Desactivar y devolver a la pool
        returnedImpact.SetActive(false);
        pool.Push(returnedImpact);
    }
}

