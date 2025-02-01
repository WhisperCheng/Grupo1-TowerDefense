using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enredaderas : StaticTower
{
    // Variables
    private float currentEnemyOriginalSpeed;
    [SerializeField] private float speedPercentage;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageInterval = 0.4f;
    [SerializeField] private float life;
    [SerializeField] private float damageOnUsed;

    private float _maxLife;
    private HealthBar _healthBar;
    private Dictionary<Collider, float> damageTimers = new Dictionary<Collider, float>();
    private Dictionary<Collider, float> originalEnemySpeeds = new Dictionary<Collider, float>();

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        damageTimers = new Dictionary<Collider, float>();
        originalEnemySpeeds = new Dictionary<Collider, float>();
        _maxLife = life;
        _healthBar = GetComponentInChildren<HealthBar>();
    }

    public override GameObject GetFromPool() { return VineTrapPool.Instance.GetVineTrap(); }
    public override void ReturnToPool()
    {
        if (_initialized && _loaded)
        {
            life = _maxLife;
            _healthBar.ResetHealthBar(); // Actualizamos la barra de salud

            List<Collider> speedKeys = new List<Collider>(originalEnemySpeeds.Keys);
            foreach (Collider col in speedKeys) // Restaurar velocidad de todos los que estén en la trampa
            {
                OnExitTrap(col);
            }

            VineTrapPool.Instance.ReturnVineTrap(this.gameObject);
        }
    }

    public override GameObject RestoreToDefault()
    {
        if (!locked)
        {
            Init();
        }
        return gameObject;
    }

    private void Update()
    {
        if (damageTimers.Keys.Count > 0 || originalEnemySpeeds.Keys.Count > 0) // Comprobar solo si hay enemigos
        {
            CheckForRemovePooledEnemies();
        }
    }

    private void CheckForRemovePooledEnemies()
    {
        List<Collider> damageKeys = new List<Collider>(damageTimers.Keys);
        List<Collider> speedKeys = new List<Collider>(originalEnemySpeeds.Keys);

        // Eliminar de damageTimers
        foreach (Collider enemy in damageKeys)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) // Si el enemigo se va a la pool 
            {                                                           // se elimina de  los diccionarios
                damageTimers.Remove(enemy);
                continue;
            } // Eliminar de la lista en caso de que se haya enviado a la pool

            if (damageTimers[enemy] > 0) // Contador para volver a hacer daño cada X segundos
            {
                damageTimers[enemy] -= Time.deltaTime;
            }
        }

        // Eliminar de originalEnemySpeeds
        foreach (Collider enemy in speedKeys)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) // Si el enemigo se va a la pool 
            {                                                           // se elimina de  los diccionarios
                originalEnemySpeeds.Remove(enemy);
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
                currentEnemyOriginalSpeed = enemy.speed;
                enemy.speed = (speedPercentage * currentEnemyOriginalSpeed) / 100f;
            }

            if (!damageTimers.ContainsKey(collision) || originalEnemySpeeds.ContainsKey(collision))
            { // Añadir enemigos a los diccionarios si no estaban antes
                //damageTimers.Add(collision, 0f);
                damageTimers.Add(collision, damageInterval);
                originalEnemySpeeds.Add(collision, currentEnemyOriginalSpeed);
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
                    damageableEntity.TakeDamage(damage); // Hacer daño
                    damageTimers[other] = cooldown;

                    life -= damageOnUsed; // Quitar vida a la trampa
                    if (life <= 0)
                    {
                        ReturnToPool();
                    }
                    else
                    {
                        _healthBar.UpdateHealthBar(_maxLife, life);
                    }
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
            bool contained = damageTimers.ContainsKey(other) || originalEnemySpeeds.ContainsKey(other);
            if (contained && enemyNavMesh != null && enemy != null)
            {
                OnExitTrap(other);
            }
        }
    }

    private void OnExitTrap(Collider col)
    {
        EnemyAI enemy = col.GetComponent<EnemyAI>();
        if (enemy) // Medida de seguridad
            enemy.speed = originalEnemySpeeds[col]; // Restaurar a velocidad original
        damageTimers.Remove(col); // Eliminar los enemigos de los diccionarios
        originalEnemySpeeds.Remove(col);
    }
}
