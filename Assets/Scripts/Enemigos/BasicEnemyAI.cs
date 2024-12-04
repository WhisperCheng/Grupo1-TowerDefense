using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyAI : EnemyAI
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        WhileWalking();
        ManageCooldown();
    }

    protected override void WhileWalking()
    {
        AnimateWalking();
        Vector3 oldDest = _destination;
        OnSearchingObjetives();
        if (oldDest != _destination) // Para comprobar que el destino sea distinto y no estar todo el rato
        {                           // asignando la misma variable
            OnAssignDestination(_destination);
        }
    }

    public override void Init()
    {
        _currentHealth = health;
        _maxHealth = health;
        _currentCooldown = cooldown;
        _healthBar = GetComponentInChildren<HealthBar>();
        agent = GetComponent<NavMeshAgent>();
        _maxSpeed = agent.speed;
        _destination = GameManager.Instance.wayPoints[_currentWaypointIndex].position;
        OnAssignDestination(_destination);
    }

    private void OnAssignDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public override void OnAttack()
    {
        // TODO: Efectos visuales al atacar
    }
    public override void Attack(IDamageable damageableEntity)
    {
        if (_canAttack)
        {
            Debug.Log("Ataque");
            damageableEntity.TakeDamage(attackDamage); // Hacer daño a la entidad Damageable
            _currentCooldown = cooldown; // Reset del cooldown
            _canAttack = false;
        }
    }

    public override void Die()
    {
        //Destroy(this.gameObject);
        _particulasMuerte.Play();
        // TODO: Devolver a la pool
    }

    private void OnTriggerStay(Collider collision)
    {

        if (collision.tag == GameManager.Instance.tagCorazonDelBosque)
        {
            IDamageable hearthEntity = collision.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
            Attack(hearthEntity);
        }
    }

    protected override void OnDamageTaken()
    {
        //Debug.Log("809");
    }
}
