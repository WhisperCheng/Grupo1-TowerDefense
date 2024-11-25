using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectilePool : MonoBehaviour
{
    // Prefab del proyectil mágico
    public GameObject magicProjectilePrefab;

    // Número de proyectiles en la pool
    public int poolSize = 50;

    // Lista de proyectiles de la pool
    private Stack<GameObject> pool;

    // Instancia singleton de la pool
    public static MagicProjectilePool Instance;

    private void Awake()
    {
        // Configurar Singleton
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
        // Inicializar la pool
        pool = new Stack<GameObject>();
        GameObject projectile = null;

        // Instanciar y desactivar todos los proyectiles al inicio
        for (int i = 0; i < poolSize; i++)
        {
            projectile = Instantiate(magicProjectilePrefab);
            projectile.SetActive(false);
            pool.Push(projectile);
        }
    }

    public GameObject GetMagicProjectile()
    {
        // Si no hay proyectiles disponibles
        GameObject newProjectile = null;

        if (pool.Count == 0)
        {
            GameObject createdProjectile = Instantiate(magicProjectilePrefab);
            return createdProjectile;
        }
        else // Tomar uno de los ya creados
        {
            newProjectile = pool.Pop();
            newProjectile.SetActive(true);
        }

        return newProjectile;
    }

    public void ReturnMagicProjectile(GameObject returnedProjectile)
    {
        // Volver a meterlo en la pool
        pool.Push(returnedProjectile);

        // Hacer que no sea visible
        returnedProjectile.SetActive(false);
    }
}


