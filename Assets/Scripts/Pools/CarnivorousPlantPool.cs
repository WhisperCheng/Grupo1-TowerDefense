using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivorousPlantPool : MonoBehaviour
{
    public GameObject carnivorousPlantPrefab;
    public int poolSize = 20;

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

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

        grandParent = GameObject.Find(grandParentName);
    }

    void Start()
    {
        SetupPool();
    }

    void SetupPool()
    {
        pool = new Stack<GameObject>();
        parent = new GameObject("CarnivorousPlant_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject plant = Instantiate(carnivorousPlantPrefab);
            plant.transform.parent = parent.transform;
            plant.SetActive(false);
            pool.Push(plant);
        }
    }

    public GameObject GetCarnivorousPlant()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de plantas carnívoras vacía.");
            GameObject newPlant = Instantiate(carnivorousPlantPrefab);
            newPlant.transform.parent = parent.transform;
            return newPlant;
        }

        GameObject plant = pool.Pop();
        IPoolable poolablePlant = plant.GetComponent<IPoolable>();
        plant.SetActive(true);
        // Solo se resetea si ya ha existido previamente
        plant = poolablePlant.RestoreToDefault();

        return plant;
    }

    public void ReturnCarnivorousPlant(GameObject returnedPlant)
    {
        // Desactivar y devolver a la pool
        returnedPlant.SetActive(false);
        pool.Push(returnedPlant);
    }
}

