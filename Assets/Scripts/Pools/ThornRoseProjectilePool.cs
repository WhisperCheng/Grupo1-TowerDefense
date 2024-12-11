using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornRoseProjectilePool : MonoBehaviour
{
    public GameObject thornRoseProjectilePrefab; // Prefab del proyectil de la rosa lanzaespinas
    public int poolSize = 20; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static ThornRoseProjectilePool Instance;

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
            GameObject projectile = Instantiate(thornRoseProjectilePrefab);
            projectile.SetActive(false);
            pool.Push(projectile);
        }
    }

    public GameObject GetThornRoseProjectile()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de proyectiles de la rosa lanzaespinas vacía.");
            SetupPool();
            //return null;
        }

        GameObject projectile = pool.Pop();
        projectile.SetActive(true);

        return projectile;
    }

    public void ReturnThornRoseProjectile(GameObject returnedProjectile)
    {
        // Desactivar y devolver a la pool
        returnedProjectile.SetActive(false);
        pool.Push(returnedProjectile);
    }
}

