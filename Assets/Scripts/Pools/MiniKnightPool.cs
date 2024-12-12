using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniKnightPool : MonoBehaviour
{
    public GameObject miniKnightPrefab; // Prefab del mini caballero
    public int poolSize = 15; // Tamaño de la pool

    GameObject parent;
    private GameObject grandParent;
    public string grandParentName = "ObjectPoolsObjects";

    private Stack<GameObject> pool;
    public static MiniKnightPool Instance;

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
        parent = new GameObject("MiniKnight_PC");
        parent.transform.parent = grandParent.transform;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject knight = Instantiate(miniKnightPrefab);
            knight.transform.parent = parent.transform;
            knight.SetActive(false);
            pool.Push(knight);
        }
    }

    public GameObject GetMiniKnight()
    {
        if (pool.Count == 0)
        {
            Debug.Log("Pool de mini caballeros vacía.");
            GameObject newKnight = Instantiate(miniKnightPrefab);
            newKnight.transform.parent = parent.transform;

           

            return newKnight;
        }

        GameObject knight = pool.Pop();
        knight.SetActive(true);

      

        return knight;
    }

    public void ReturnMiniKnight(GameObject returnedKnight)
    {
        // Llamar al método ResetKnight para limpiar el estado del caballero
        returnedKnight.GetComponent<MiniKnightAI>().ResetKnight();

        // Desactivar y devolver a la pool
        returnedKnight.SetActive(false);

        // Agregar de vuelta a la pool
        pool.Push(returnedKnight);
    }
}
