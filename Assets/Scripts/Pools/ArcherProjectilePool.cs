using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherProjectilePool : MonoBehaviour
{
    public GameObject archerProjectilePrefab; // Prefab del proyectil del arquero (por ejemplo, una flecha)
    public int poolSize = 30; // Tamaño de la pool

    private Stack<GameObject> pool;
    public static ArcherProjectilePool Instance;

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

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
        parent = new GameObject("ArcherProjectile_PC");
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(archerProjectilePrefab);
            projectile.transform.parent = parent.transform;
            projectile.SetActive(false);
            pool.Push(projectile);
        }
    }

    public GameObject GetArcherProjectile()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de proyectiles del arquero vacía.");
            GameObject newProjectile = Instantiate(archerProjectilePrefab);
            newProjectile.transform.parent = parent.transform;
            return newProjectile;
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
