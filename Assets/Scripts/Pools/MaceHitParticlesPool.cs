using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceHitParticlesPool : MonoBehaviour
{
    public GameObject particlesPrefab; // Prefab de las part�culas de muerte de enemigos
    public int poolSize = 20;

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool;
    public static MaceHitParticlesPool Instance;

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
        parent = new GameObject("MaceHitParticles_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {  // Crear nuevas part�culas si la pool est� vac�a
            GameObject particles = Instantiate(particlesPrefab);
            particles.transform.parent = parent.transform;
            particles.SetActive(false);
            pool.Push(particles);
        }
    }

    public GameObject GetMaceHitParticles()
    {
        if (pool.Count == 0)
        {
            GameObject newParticles = Instantiate(particlesPrefab);
            newParticles.transform.parent = parent.transform;
            return newParticles;
        }

        GameObject particles = pool.Pop();
        particles.SetActive(true);
        return particles;
    }

    public void ReturntMaceHitParticles(GameObject returnedParticles)
    {
        // Desactivar y devolver a la pool
        returnedParticles.SetActive(false);
        pool.Push(returnedParticles);
    }
}

