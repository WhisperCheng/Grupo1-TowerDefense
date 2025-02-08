using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enredaderas : StaticTower, IDamageable
{
    // Variables
    [SerializeField] private float speedPercentage;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageInterval = 0.4f;
    [SerializeField] private float life;
    [SerializeField] private float damageOnUsed;

    private float _maxLife;
    private HealthBar _healthBar;
    private Dictionary<Collider, float> damageTimers = new Dictionary<Collider, float>();

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();
        damageTimers = new Dictionary<Collider, float>();
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

            VineTrapPool.Instance.ReturnVineTrap(this.gameObject);

            List<Collider> damageKeys = new List<Collider>(damageTimers.Keys);
            foreach (Collider col in damageKeys) // Restaurar velocidad de todos los que estén en la trampa
            {
                OnExitTrap(col);
            }
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
        if (damageTimers.Keys.Count > 0) // Comprobar solo si hay enemigos
        {
            CheckForRemovePooledEnemies();
        }
    }

    private void CheckForRemovePooledEnemies()
    {
        List<Collider> damageKeys = new List<Collider>(damageTimers.Keys);

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
            else
            {
                damageTimers[enemy] = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(GameManager.Instance.tagEnemigos) && !collision.isTrigger)
        {
            ChangeEnemySpeed(collision);
            StartCoroutine(CheckForDelayedSpeedChange(collision));
        }
    }

    // Esto sirve para realentizar a los enemigos con cierto delay una vez entran a una nueva trampa
    // Es útil ya que sin ese delay la velocidad al salir de una trampa anterior que estuviera muy pegada
    // a la siguiente haría que la velocidad del enemigo volviera a ser la máxima en lugar de la realentizada
    private IEnumerator CheckForDelayedSpeedChange(Collider col)
    {
        for (int i = 0; i < 7; i++)
        {
            yield return new WaitForSeconds(0.1f);
            if (damageTimers.ContainsKey(col)) // Solo entra si el enemigo está contenido en el diccionario/lista
                ChangeEnemySpeed(col);
        }
    }
    private void ChangeEnemySpeed(Collider collision)
    {
        NavMeshAgent enemyNavMesh = collision.GetComponent<NavMeshAgent>();
        EnemyAI enemy = collision.GetComponent<EnemyAI>();
        if (enemyNavMesh != null && enemy != null)
        {
            enemy.speed = (speedPercentage * enemy.MaxSpeed) / 100f;
        }

        if (!damageTimers.ContainsKey(collision))
        { // Añadir enemigos a los diccionarios si no estaban antes
            damageTimers.Add(collision, damageInterval);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(GameManager.Instance.tagEnemigos) && !other.isTrigger)
        {
            if (damageTimers.ContainsKey(other))
            {
                IDamageable damageableEntity = other.GetComponent(typeof(IDamageable)) as IDamageable;
                if (damageTimers[other] <= 0f && damageableEntity != null)
                {
                    damageableEntity.TakeDamage(damage); // Hacer daño
                    damageTimers[other] = cooldown;
                    TakeDamage(damageOnUsed);
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
            bool contained = damageTimers.ContainsKey(other);
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
            enemy.speed = enemy.MaxSpeed; // Restaurar a velocidad original
        damageTimers.Remove(col); // Eliminar los enemigos de los diccionarios
    }

    public void Die() { ReturnToPool(); }

    public void TakeDamage(float damageAmount)
    {
        life -= damageAmount; // Quitar vida a la trampa
        if (life <= 0)
        {
            Die();
        }
        else
        {
            _healthBar.UpdateHealthBar(_maxLife, life);
        }
    }

    public float GetHealth() { return life; }

    public float GetMaxHealth() { return _maxLife; }
}
