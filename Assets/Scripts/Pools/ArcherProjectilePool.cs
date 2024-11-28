using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherProjectilePool : MonoBehaviour
{
    public GameObject archerProjectilePrefab; // Prefab del proyectil del arquero (por ejemplo, una flecha)
    public int poolSize = 30; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static ArcherProjectilePool Instance;

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
            GameObject projectile = Instantiate(archerProjectilePrefab);
            projectile.SetActive(false);
            pool.Push(projectile);
        }
    }

    public GameObject GetArcherProjectile()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de proyectiles del arquero vacía.");
            return null;
        }

        GameObject projectile = pool.Pop();
        projectile.SetActive(true);

        return projectile;
    }

    public void ReturnArcherProjectile(GameObject returnedProjectile)
    {
        // Desactivar y devolver a la pool
        returnedProjectile.SetActive(false);
        pool.Push(returnedProjectile);
    }
}
