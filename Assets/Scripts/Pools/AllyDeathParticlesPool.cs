using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyDeathParticlesPool : MonoBehaviour
{
    public GameObject allyDeathParticlesPrefab; // Prefab de las partículas de muerte de aliados
    public int poolSize = 20;

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool;
    public static AllyDeathParticlesPool Instance;

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
            Debug.LogError($"No se encontró un objeto llamado '{grandParentName}' en la escena.");
        }
    }

    void Start()
    {
        SetupPool();
    }

    void SetupPool()
    {
        pool = new Stack<GameObject>();
        parent = new GameObject("AllyDeathParticles_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject particles = Instantiate(allyDeathParticlesPrefab);
            particles.transform.parent = parent.transform;
            particles.SetActive(false);
            pool.Push(particles);
        }
    }

    public GameObject GetAllyDeathParticles()
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("Pool de partículas de muerte de aliados vacía. Creando nuevo objeto de partículas.");
            GameObject newParticles = Instantiate(allyDeathParticlesPrefab);
            newParticles.transform.parent = parent.transform;
            return newParticles;
        }

        GameObject particles = pool.Pop();
        particles.SetActive(true);
        return particles;
    }

    public void ReturnAllyDeathParticles(GameObject returnedParticles)
    {
        // Desactivar y devolver a la pool
        returnedParticles.SetActive(false);
        pool.Push(returnedParticles);
    }
}
