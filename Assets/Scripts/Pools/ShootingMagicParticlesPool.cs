using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingMagicParticlesPool : MonoBehaviour
{
    public GameObject magicParticlePrefab;
    public int poolSize = 20;

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool;
    public static ShootingMagicParticlesPool Instance;

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
        parent = new GameObject("MagicShootingParticles_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject impact = Instantiate(magicParticlePrefab);
            impact.transform.parent = parent.transform;
            impact.SetActive(false);
            pool.Push(impact);
        }
    }

    public GameObject GetMagicImpact()
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("Pool de impactos vac�a. Creando nuevo objeto de impacto.");
            GameObject newImpact = Instantiate(magicParticlePrefab);
            newImpact.transform.parent = parent.transform; 
            return newImpact;
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

