using UnityEngine;
using UnityEngine.AI;

public class MiniKnightAI : MonoBehaviour
{
    private EnemyAI enemyAI;  // Referencia a la IA del enemigo
   
    private float maxHealth; // Salud máxima del caballero

    void Awake()
    {
        // Obtener las referencias necesarias
        enemyAI = GetComponent<EnemyAI>();
        

        // Guardar la salud máxima del caballero (se toma del valor inicial de la variable health en EnemyAI)
        maxHealth = enemyAI.health;
    }

    void Update()
    {
        // Comprobar si la salud del enemigo ha llegado a cero
        if (enemyAI.GetHealth() <= 0)
        {
            // Si la salud es menor o igual a 0, se activa el método para devolverlo a la pool
            //Die();
        }
    }

    // Este método se ejecuta cuando el caballero reciba daño y su salud llegue a 0
    private void Die()
    {
        // Aquí podrías agregar efectos de muerte, animaciones, etc.

        // Desactivamos el NavMeshAgent y la IA del enemigo
        //enemyAI.GetComponent<NavMeshAgent>().enabled = false;
        //enemyAI.enabled = false;

        // Llamamos a la pool para devolver al caballero
        //MiniKnightPool.Instance.ReturnMiniKnight(gameObject);

        // Si deseas agregar efectos visuales de muerte, partículas, etc., puedes hacerlo aquí.
    }

    // Este método se utiliza cuando el caballero es reciclado desde la pool
    public void ResetKnight()
    {
        // Volver a activar el NavMeshAgent y la IA del enemigo
        //enemyAI.GetComponent<NavMeshAgent>().enabled = true;
        //enemyAI.enabled = true;


        // Restaurar la salud del caballero al valor máximo
        //enemyAI.health = maxHealth;  // Asegúrate de asignar la salud máxima directamente
        //enemyAI.GetComponent<BasicEnemyAI>().ResetValues(); // Aseguramos que _currentHealth también se actualice

        //enemyAI._agent.SetDestination(GameManager.Instance.wayPoints[0].transform.position);

        // Si necesitas reiniciar animaciones o efectos, también puedes hacerlo aquí
        //if (enemyAI.animatorController != null)
        //{
            // Reiniciar animaciones si es necesario
            //enemyAI.animatorController.SetTrigger("Reset"); // Asume que tienes un trigger para resetear animaciones
        //}
    }
}
