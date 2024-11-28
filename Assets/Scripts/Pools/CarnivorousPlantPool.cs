using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivorousPlantPool : MonoBehaviour
{
    public GameObject carnivorousPlantPrefab;
    public int poolSize = 20;

    private Stack<GameObject> pool;
    public static CarnivorousPlantPool Instance;

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
            GameObject plant = Instantiate(carnivorousPlantPrefab);
            plant.SetActive(false);
            pool.Push(plant);
        }
    }

    public GameObject GetCarnivorousPlant()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de plantas carnívoras vacía.");
            return null;
        }

        GameObject plant = pool.Pop();
        plant.SetActive(true);

        return plant;
    }

    public void ReturnCarnivorousPlant(GameObject returnedPlant)
    {
        // Desactivar y devolver a la pool
        returnedPlant.SetActive(false);
        pool.Push(returnedPlant);
    }
}

