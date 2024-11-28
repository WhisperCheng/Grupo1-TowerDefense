using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSlashProjectilePool : MonoBehaviour
{
    public GameObject poisonSlashProjectilePrefab; // Prefab del proyectil del tajo venenoso
    public int poolSize = 20; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static PoisonSlashProjectilePool Instance;

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
            GameObject projectile = Instantiate(poisonSlashProjectilePrefab);
            projectile.SetActive(false);
            pool.Push(projectile);
        }
    }

    public GameObject GetPoisonSlashProjectile()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de proyectiles del tajo venenoso vacía.");
            return null;
        }

        GameObject projectile = pool.Pop();
        projectile.SetActive(true);

        return projectile;
    }

    public void ReturnPoisonSlashProjectile(GameObject returnedProjectile)
    {
        // Desactivar y devolver a la pool
        returnedProjectile.SetActive(false);
        pool.Push(returnedProjectile);
    }
}
