using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThornRoseProjectilePool : MonoBehaviour
{
    public GameObject thornRoseProjectilePrefab; // Prefab del proyectil de la rosa lanzaespinas
    public int poolSize = 20; // Tamaño de la pool

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool; 
    //private Stack pool; 
    // https://learn.microsoft.com/en-us/dotnet/api/system.collections.stack.synchronized?view=net-9.0
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
        grandParent = GameObject.Find(grandParentName);
    }

    void Start()
    {
        SetupPool();
    }

    void SetupPool()
    {
        pool = new Stack<GameObject>();
        //pool = Stack.Synchronized(new Stack());
        parent = new GameObject("ThornRoseProjectile_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(thornRoseProjectilePrefab);
            projectile.transform.parent = parent.transform;
            projectile.SetActive(false);
            pool.Push(projectile);
        }
    }

    public GameObject GetThornRoseProjectile()
    {
        if (pool.Count == 0)
        {
            return CreateNewProyectile();
        }

        GameObject proyectile = pool.Pop();
        /*GameObject projectile = pool.ToArray().LastOrDefault(x => !x.activeSelf);
        if(projectile != null) // Si no hay proyectiles activos se crea uno nuevo
        {
            pool.TryPop(out projectile); // Eliminar objeto de la pool
        } else
        {
            return CreateNewProyectile();
        }*/
        proyectile.SetActive(true);
        proyectile.gameObject.GetComponent<RangedTowerProyectile>().PopFromPool();
        return proyectile;
    }

    private GameObject CreateNewProyectile()
    {
        Debug.Log("Pool de proyectiles de la rosa lanzaespinas vacía.");
        GameObject newProjectile = Instantiate(thornRoseProjectilePrefab);
        newProjectile.transform.parent = parent.transform;
        newProjectile.SetActive(true);
        return newProjectile;
    }

    public void ReturnThornRoseProjectile(GameObject returnedProjectile)
    {
        // Desactivar y devolver a la pool
        returnedProjectile.SetActive(false);
        pool.Push(returnedProjectile);
    }
}

