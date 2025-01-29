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
            Debug.LogError($"No se encontró un objeto llamado '{grandParentName}' en la escena.");
        }
    }

    private void Start()
    {
        SetupPool();
    }

    private void SetupPool()
    {

        pool = new Stack<GameObject>();
        parent = new GameObject("MagicShootingParticles_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject shootingParticle = Instantiate(magicParticlePrefab);
            shootingParticle.transform.parent = parent.transform;
            shootingParticle.SetActive(false);
            pool.Push(shootingParticle);
        }
    }

    public GameObject GetMagicShootingParticle()
    {
        if (pool.Count == 0)
        {
            GameObject newShootingParticle = Instantiate(magicParticlePrefab);
            newShootingParticle.transform.parent = parent.transform; 
            return newShootingParticle;
        }

        GameObject shootingParticle = pool.Pop();
        shootingParticle.SetActive(true);
        return shootingParticle;
    }

    public void ReturnMagicShootingParticle(GameObject returnedShootingParticle)
    {
        // Desactivar y devolver a la pool
        returnedShootingParticle.SetActive(false);
        pool.Push(returnedShootingParticle);
    }
}

