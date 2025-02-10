using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonParticleImpactPool : MonoBehaviour
{
    public GameObject poisonYewImpactParticlesPrefab; // Prefab de las partículas de impacto del tajo venenoso
    public int poolSize = 20;

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool;
    public static PoisonParticleImpactPool Instance;

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
        parent = new GameObject("PoisonYewParticles_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject particles = Instantiate(poisonYewImpactParticlesPrefab);
            particles.transform.parent = parent.transform;
            particles.SetActive(false);
            pool.Push(particles);
        }
    }

    public GameObject GetPoisonYewImpactParticles()
    {
        if (pool.Count == 0)
        {
            GameObject newParticles = Instantiate(poisonYewImpactParticlesPrefab);
            newParticles.transform.parent = parent.transform;
            return newParticles;
        }

        GameObject particles = pool.Pop();
        particles.SetActive(true);
        return particles;
    }

    public void ReturnPoisonYewImpactParticles(GameObject returnedParticles)
    {
        // Desactivar y devolver a la pool
        returnedParticles.SetActive(false);
        pool.Push(returnedParticles);
    }
}

