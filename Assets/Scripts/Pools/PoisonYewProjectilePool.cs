using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonYewProjectilePool : MonoBehaviour
{
    public GameObject poisonYewProjectilePrefab; // Prefab del proyectil del tejo venenoso
    public int poolSize = 20; // Tamaño de la pool 

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool;
    public static PoisonYewProjectilePool Instance;

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
        parent = new GameObject("PoisonYewProjectile_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(poisonYewProjectilePrefab);
            projectile.transform.parent = parent.transform;
            projectile.SetActive(false);
            pool.Push(projectile);
        }
    }

    public GameObject GetPoisonYewProjectile()
    {
        if (pool.Count == 0)
        {
            GameObject newProjectile = Instantiate(poisonYewProjectilePrefab);
            newProjectile.transform.parent = parent.transform;
            newProjectile.SetActive(true);
            return newProjectile;
        }

        GameObject proyectile = pool.Pop();
        proyectile.SetActive(true);
        proyectile.gameObject.GetComponent<RangedTowerProyectile>().PopFromPool();

        return proyectile;
    }

    public void ReturnPoisonYewProjectile(GameObject returnedProjectile)
    {
        // Desactivar y devolver a la pool
        returnedProjectile.SetActive(false);
        pool.Push(returnedProjectile);
    }
}
