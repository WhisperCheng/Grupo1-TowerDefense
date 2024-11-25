using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicImpactPool : MonoBehaviour
{
    // Prefab del efecto de impacto mágico
    public GameObject magicImpactPrefab;

    // Número de impactos en la pool
    public int poolSize = 30;

    // Lista de impactos de la pool
    private Stack<GameObject> pool;

    // Instancia singleton de la pool
    public static MagicImpactPool Instance;

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
        GameObject impact = null;

        // Instanciar y desactivar todos los impactos al inicio
        for (int i = 0; i < poolSize; i++)
        {
            impact = Instantiate(magicImpactPrefab);
            impact.SetActive(false);
            pool.Push(impact);
        }
    }

    public GameObject GetMagicImpact()
    {
        // Si no hay impactos disponibles
        GameObject newImpact = null;

        if (pool.Count == 0)
        {
            GameObject createdImpact = Instantiate(magicImpactPrefab);
            return createdImpact;
        }
        else // Tomar uno de los ya creados
        {
            newImpact = pool.Pop();
            newImpact.SetActive(true);
        }

        return newImpact;
    }

    public void ReturnMagicImpact(GameObject returnedImpact)
    {
        // Volver a meterlo en la pool
        pool.Push(returnedImpact);

        // Hacer que no sea visible
        returnedImpact.SetActive(false);
    }
}

