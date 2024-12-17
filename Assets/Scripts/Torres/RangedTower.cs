using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedTower : Tower
{
    [Header("Vida")] // Vida
    public float health;
    protected HealthBar _healthBar;
    [SerializeField] protected ParticleSystem _particulasMuerte;
    [SerializeField] protected ParticleSystem _particulasGolpe;

    [Header("Ataque")] //Ataque
    public float attackDamage;
    public float cooldown = 1f;

    //[Header("Animaciones")]
    protected Animator animator;

    [Header("Proyectil")]
    //[SerializeField] protected GameObject proyectile;
    [SerializeField] protected Transform shooterSource;
    [Range(0,100)]
    [SerializeField] protected float shootingSpeed;

    protected float _currentHealth;
    protected float _maxHealth;
    protected float _currentCooldown = 0f;
    protected float _maxSpeed;

    protected bool _attackMode = false;
    protected bool _canAttack = true;

    private void Start()
    {
        Init();
    }

    protected void Update()
    {
        if (!_locked)
        {
            //ManageCooldown(); // TODO: Implementar cooldown
            EnemyDetection();
            LookRotation();
        }
    }

    /*protected void ManageCooldown()
    {
        _currentCooldown -= Time.deltaTime;
        if (!_canAttack && _currentCooldown <= 0)
        {
            _canAttack = true;
            _currentCooldown = 0;
        }
    }*/

    public override void ReturnToPool()
    {
        _locked = true;
        _currentHealth = health; // Restaurar la salud del caballero al valor máximo
        _healthBar = GetComponentInChildren<HealthBar>();
        _healthBar.ResetHealthBar(); // Actualizamos la barra de salud
        // Importante: usar el metodo base. para luego hacer override en cada instancia y añadir el retorno a la pool
    }

    public override GameObject RestoreToDefault()
    {
        if (!_locked)
        {// Si ya ha sido enviado previamente a la pool, se resetean los valores por defecto
            Init();
            enabled = true;
            _attackMode = false;
            _canAttack = false;
            animator.SetBool("AttackMode", false); // Dejar de reproducir animación de atacar
        }
        return this.gameObject;
    }


    protected override void EnemyDetection()
    {
        //currentTargets.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, _enemyMask);
        if (colliders.Length > 0)
        {

            foreach (Collider collider in colliders)
            {
                if (!_hasEnemyAssigned) // Si no tiene ningún enemigo asignado, se le asigna uno
                {
                    //currentTargets.Add(collider.gameObject);
                    currentTarget = collider.gameObject;
                    _hasEnemyAssigned = true;
                    _attackMode = true;
                    animator.SetBool("AttackMode", true);
                    break;
                }
            }
        }
        else // Si no se han detectado enemigos, el target actual es nulo y no le hace focus a nada
        {
            //if (currentTarget != null)
            //{
                currentTarget = null;
                _hasEnemyAssigned = false;
                _attackMode = false;
                animator.SetBool("AttackMode", false);
            //}
        }

        if (_hasEnemyAssigned) // Si tiene un enemigo asignado que esé dentro del rango, empieza a atacar
        {
            OnAttack();
        }
    }

    /*public override void Attack(IDamageable damageableEntity)
    {
        if (_canAttack)
        {
            //damageableEntity.TakeDamage(attackDamage); // Hacer daño a la entidad Damageable
            _currentCooldown = cooldown; // Reset del cooldown
            _canAttack = false;
            Debug.Log("AtaqueRosa");
        }
    }*/

    public override void Init()
    {
        animator = GetComponent<Animator>();
        _currentHealth = health;
        _maxHealth = health;
        _currentCooldown = cooldown;
        _healthBar = GetComponentInChildren<HealthBar>();
        _enemyMask = 1 << GameManager.Instance.layerEnemigos;
    }

    public override void OnAttack()
    {
        if (_canAttack && _attackMode && !_locked && currentTarget != null)
        {
            _currentCooldown = cooldown; // Reset del cooldown
            animator.SetTrigger("Attack");
        }
        else
        {
            _attackMode = false;
        }

        /*if (!_canAttack || currentTarget == null)
        {
            //animator.SetBool("AttackMode", false);
        }*/
    }



    public abstract void ShootProyectileEvent();

    public override void TakeDamage(float damageAmount)
    {
        // Dañar torre
        _currentHealth -= damageAmount;
        OnDamageTaken();

        // Actualizar barra de vida
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    protected override void OnDamageTaken()
    {
        //
    }

    public override float GetHealth()
    {
        return _currentHealth;
    }
}
