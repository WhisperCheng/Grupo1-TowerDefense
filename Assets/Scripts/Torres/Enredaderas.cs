using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enredaderas : StaticTower
{
    // Variables
    private float originalSpeed;
    [SerializeField] private float speedPercentage;
    [SerializeField] private float damage = 10f;
    private Dictionary<Collider, float> damageTimers = new Dictionary<Collider, float>();

    public override GameObject GetFromPool() { return VineTrapPool.Instance.GetVineTrap(); }
    public override void ReturnToPool() { VineTrapPool.Instance.ReturnVineTrap(this.gameObject); }

    public override GameObject RestoreToDefault()
    {
        damageTimers = new Dictionary<Collider, float>();
        return gameObject;
    }

    private void Update()
    {
        List<Collider> keys = new List<Collider>(damageTimers.Keys);

        foreach (Collider enemy in keys)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy)
            {
                damageTimers.Remove(enemy);
                continue;
            } // Eliminar de la lista en caso de que se haya enviado de la pool

            if (damageTimers[enemy] > 0)
            {
                damageTimers[enemy] -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(GameManager.Instance.tagEnemigos) && !collision.isTrigger)
        {
            NavMeshAgent enemyNavMesh = collision.GetComponent<NavMeshAgent>();
            EnemyAI enemy = collision.GetComponent<EnemyAI>();
            if (enemyNavMesh != null && enemy != null)
            {
                originalSpeed = enemy.speed;
                enemy.speed = (speedPercentage * originalSpeed) / 100f;
            }

            if (!damageTimers.ContainsKey(collision))
            {
                //damageTimers.Add(collision, 0f);
                damageTimers.Add(collision, enemy.speed);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameManager.Instance.tagEnemigos) && !other.isTrigger)
        {
            if (damageTimers.ContainsKey(other) && damageTimers[other] <= 0f)
            {
                IDamageable damageableEntity = other.GetComponent(typeof(IDamageable)) as IDamageable;
                if (damageableEntity != null)
                {
                    damageableEntity.TakeDamage(damage);
                    damageTimers[other] = cooldown;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameManager.Instance.tagEnemigos) && !other.isTrigger)
        {
            NavMeshAgent enemyNavMesh = other.GetComponent<NavMeshAgent>();
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemyNavMesh != null && enemy != null)
            {
                enemy.speed = originalSpeed;
            }
            if (damageTimers.ContainsKey(other))
            {
                damageTimers.Remove(other);
            }
        }
    }
}
