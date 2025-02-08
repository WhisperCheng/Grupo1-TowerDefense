using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StudioEventEmitter))]


public class RunaStun : StaticTower
{
    private StudioEventEmitter emitter;
    private HealthBar _healthBar;

    private bool canAttack = true;
    private bool lifeIsZero = false;

    [Header("Parámetros Runa")]
    [SerializeField] private float life;
    private float _maxLife;
    [SerializeField] private float duration = 3f;
    [SerializeField] private float damageOnUsed;

    private Dictionary<NavMeshAgent, float> stunnedEnemies = new Dictionary<NavMeshAgent, float>();

    public override void Init()
    {
        base.Init();
        _maxLife = life;
        _healthBar = GetComponentInChildren<HealthBar>();
    }
    void Start()
    {
        Init();
    }
    void Update()
    {

    }
    private void RemoveLife()
    {
        life -= damageOnUsed; // Quitar vida a la trampa
    }
    private void CheckIfLifeIsZero() // Si la vida es cero se devuelve a la pool
    {
        if (life <= 0)
        {
            lifeIsZero = true;
        }
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
                RemoveLife();
                _healthBar.UpdateHealthBar(_maxLife, life);
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
        Debug.Log("a");
        //Debug.Log($"Enemigo {enemyNavMesh.name} stunneado");

        yield return new WaitForSeconds(duration);

        // Restaurar la velocidad original del enemigo
        if (stunnedEnemies.ContainsKey(enemyNavMesh))
        {
            enemy.speed = stunnedEnemies[enemyNavMesh];
            stunnedEnemies.Remove(enemyNavMesh);
            //Debug.Log($"Enemigo {enemyNavMesh.name} destunneado");
        }
        CheckIfLifeIsZero();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (gameObject.activeInHierarchy) // Solo si está activa la runa empieza a parar
            if (collision.CompareTag(GameManager.Instance.tagEnemigos) && canAttack && !collision.isTrigger)
            {
                canAttack = false;
                if (!lifeIsZero) StartCoroutine(ResetCooldown());
                if (!lifeIsZero) EnemyDetection();
                emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.runeStun, this.gameObject);
                emitter.Play();
            }
    }
    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        if (lifeIsZero) ReturnToPool();
        if (!lifeIsZero) canAttack = true;
        //yield return new WaitForSeconds(0.01f);
    }
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    public override void ReturnToPool()
    {
        if (_initialized && _loaded)
        {
            canAttack = true;
            life = _maxLife;
            _healthBar.ResetHealthBar(); // Actualizamos la barra de salud
            RuneTrapPool.Instance.ReturnRuneTrap(gameObject);
        }
    }
    public override GameObject RestoreToDefault()
    {
        if (!locked) { lifeIsZero = false; Init(); }
        return gameObject;
    }
    public override GameObject GetFromPool() { return RuneTrapPool.Instance.GetRuneTrap(); }
}
