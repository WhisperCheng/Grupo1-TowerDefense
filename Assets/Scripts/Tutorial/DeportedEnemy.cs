using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeportedEnemy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Verifica si el objeto que colisionó tiene la etiqueta "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Contacto enemigo en el puente");
            // Obtiene la referencia al singleton de MiniKnightPool
            MiniKnightPool pool = MiniKnightPool.Instance;

            if (pool != null)
            {
                Debug.Log("Enemigo retornado");
                // Llama al método para devolver el enemigo a la pool
                pool.ReturnMiniKnight(collision.gameObject);
            }
            else
            {
                Debug.LogError("MiniKnightPool no está inicializado.");
            }
        }
    }
}
