using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunaStun : MonoBehaviour
{
    private bool canAttack = true;
    [SerializeField] private float duration = 3f;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private float range = 10f;

    private Dictionary<NavMeshAgent, float> stunnedEnemies = new Dictionary<NavMeshAgent, float>();

    void Update()
    {
        
    }

    private void EnemyDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        foreach (Collider collider in colliders)
        {
            NavMeshAgent enemyNavMesh = collider.GetComponent<NavMeshAgent>();
            EnemyAI enemy = collider.GetComponent<EnemyAI>();
            if (enemyNavMesh != null && enemy != null && !stunnedEnemies.ContainsKey(enemyNavMesh))
            {
                StartCoroutine(StunCoroutine(enemyNavMesh, enemy));
            }
        }
    }

    private IEnumerator StunCoroutine(NavMeshAgent enemyNavMesh, EnemyAI enemy)
    {
        // Guardar la velocidad original del enemigo
        if (!stunnedEnemies.ContainsKey(enemyNavMesh))
        {
            stunnedEnemies[enemyNavMesh] = enemy.speed;
        }

        enemy.speed = 0; // Aplicar el stun
        Debug.Log($"Enemigo {enemyNavMesh.name} stunneado");

        yield return new WaitForSeconds(duration);

        // Restaurar la velocidad original del enemigo
        if (stunnedEnemies.ContainsKey(enemyNavMesh))
        {
            enemy.speed = stunnedEnemies[enemyNavMesh];
            stunnedEnemies.Remove(enemyNavMesh);
            Debug.Log($"Enemigo {enemyNavMesh.name} destunneado");
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(GameManager.Instance.tagEnemigos) && canAttack && !collision.isTrigger)
        {
            canAttack = false; 
            EnemyDetection();
            StartCoroutine(ResetCooldown());
        }
    }

    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
