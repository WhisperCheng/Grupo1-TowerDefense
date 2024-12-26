using UnityEngine;
using UnityEngine.AI;

public class MiniKnightAI : MonoBehaviour
{
    private EnemyAI enemyAI;  // Referencia a la IA del enemigo
   
    private float maxHealth; // Salud m�xima del caballero

    void Awake()
    {
        // Obtener las referencias necesarias
        enemyAI = GetComponent<EnemyAI>();
        

        // Guardar la salud m�xima del caballero (se toma del valor inicial de la variable health en EnemyAI)
        maxHealth = enemyAI.health;
    }

    void Update()
    {
        // Comprobar si la salud del enemigo ha llegado a cero
        if (enemyAI.GetHealth() <= 0)
        {
            // Si la salud es menor o igual a 0, se activa el m�todo para devolverlo a la pool
            //Die();
        }
    }

    // Este m�todo se ejecuta cuando el caballero reciba da�o y su salud llegue a 0
    private void Die()
    {
        // Aqu� podr�as agregar efectos de muerte, animaciones, etc.

        // Desactivamos el NavMeshAgent y la IA del enemigo
        //enemyAI.GetComponent<NavMeshAgent>().enabled = false;
        //enemyAI.enabled = false;

        // Llamamos a la pool para devolver al caballero
        //MiniKnightPool.Instance.ReturnMiniKnight(gameObject);

        // Si deseas agregar efectos visuales de muerte, part�culas, etc., puedes hacerlo aqu�.
    }

    // Este m�todo se utiliza cuando el caballero es reciclado desde la pool
    public void ResetKnight()
    {
        // Volver a activar el NavMeshAgent y la IA del enemigo
        //enemyAI.GetComponent<NavMeshAgent>().enabled = true;
        //enemyAI.enabled = true;


        // Restaurar la salud del caballero al valor m�ximo
        //enemyAI.health = maxHealth;  // Aseg�rate de asignar la salud m�xima directamente
        //enemyAI.GetComponent<BasicEnemyAI>().ResetValues(); // Aseguramos que _currentHealth tambi�n se actualice

        //enemyAI._agent.SetDestination(GameManager.Instance.wayPoints[0].transform.position);

        // Si necesitas reiniciar animaciones o efectos, tambi�n puedes hacerlo aqu�
        //if (enemyAI.animatorController != null)
        //{
            // Reiniciar animaciones si es necesario
            //enemyAI.animatorController.SetTrigger("Reset"); // Asume que tienes un trigger para resetear animaciones
        //}
    }
}
