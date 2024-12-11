using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectilePool : MonoBehaviour
{
    // Prefab del proyectil mágico
    public GameObject magicProjectilePrefab;

    // Número de proyectiles en la pool
    public int poolSize = 20;

    // Lista de proyectiles de la pool
    private Stack<GameObject> pool;

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

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
        // Inicializar la pool
        pool = new Stack<GameObject>();
        GameObject projectile = null;
        parent = new GameObject("MagicProjectilePoolContainer");
        parent.transform.parent = grandParent.transform;
        // Instanciar y desactivar todos los proyectiles al inicio
        for (int i = 0; i < poolSize; i++)
        {
            projectile = Instantiate(magicProjectilePrefab);
           
            
            projectile.transform.parent = parent.transform;
            projectile.SetActive(false);
            pool.Push(projectile);
        }
    }

    public GameObject GetMagicProjectile(Vector3 vector3, Quaternion quaternion)
    {
        GameObject newProjectile;

        if (pool.Count == 0)
        {
            // Si no hay proyectiles, instanciar uno nuevo
            newProjectile = Instantiate(magicProjectilePrefab, vector3, quaternion);
            newProjectile.transform.parent = parent.transform;
        }
        else
        {
            // Tomar uno de la pool
            newProjectile = pool.Pop();
            newProjectile.SetActive(true);
        }

        // Configurar posición y rotación
        newProjectile.transform.position = vector3;
        newProjectile.transform.rotation = quaternion;

        return newProjectile;
    }

    public void ReturnMagicProjectile(GameObject returnedProjectile)
    {
        //Limpiar los trails
        TrailRenderer trail = returnedProjectile.GetComponentInChildren<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear();
            trail.enabled = false;
            trail.enabled = true;
        }

        // Desactivar el proyectil
        returnedProjectile.SetActive(false);

        // Volver a meterlo en la pool
        pool.Push(returnedProjectile);
    }
}


