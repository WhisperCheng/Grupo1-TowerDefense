using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathParticlesPool : MonoBehaviour
{
    public GameObject enemyDeathParticlesPrefab; // Prefab de las partículas de muerte de enemigos
    public int poolSize = 20;

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool;
    public static EnemyDeathParticlesPool Instance;

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
        parent = new GameObject("EnemyDeathParticles_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject particles = Instantiate(enemyDeathParticlesPrefab);
            particles.transform.parent = parent.transform;
            particles.SetActive(false);
            pool.Push(particles);
        }
    }

    public GameObject GetEnemyDeathParticles()
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("Pool de partículas de muerte de enemigos vacía. Creando nuevo objeto de partículas.");
            GameObject newParticles = Instantiate(enemyDeathParticlesPrefab);
            newParticles.transform.parent = parent.transform;
            return newParticles;
        }

        GameObject particles = pool.Pop();
        particles.SetActive(true);
        return particles;
    }

    public void ReturnEnemyDeathParticles(GameObject returnedParticles)
    {
        // Desactivar y devolver a la pool
        returnedParticles.SetActive(false);
        pool.Push(returnedParticles);
    }
}

