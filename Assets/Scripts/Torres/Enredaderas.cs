using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enredaderas : MonoBehaviour
{
    // Variables
    private float originalSpeed;
    [SerializeField] private float speedPercentage;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private float damage = 10f;
    private Dictionary<Collider, float> damageTimers = new Dictionary<Collider, float>();

    private void Update()
    {
        List<Collider> keys = new List<Collider>(damageTimers.Keys);

        foreach (Collider enemy in keys)
        {
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
            if (enemyNavMesh != null)
            {
                originalSpeed = enemyNavMesh.speed;
                enemyNavMesh.speed = (speedPercentage * originalSpeed) / 100f;
            }

            if (!damageTimers.ContainsKey(collision))
            {
                damageTimers.Add(collision, 0f);
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
            if (enemyNavMesh != null)
            {
                enemyNavMesh.speed = originalSpeed;
            }
            if (damageTimers.ContainsKey(other))
            {
                damageTimers.Remove(other);
            }
        }
    }
}
