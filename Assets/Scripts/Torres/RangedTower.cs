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
    public Animator animator;

    [Header("Proyectil")]
    [SerializeField] protected GameObject proyectile;

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
        EnemyDetection();
        ManageCooldown();
        if (PlaceManager.Instance.objetoSiendoArrastrado == false)
        {
            LookRotation();
        }
    }

    protected void ManageCooldown()
    {
        _currentCooldown -= Time.deltaTime;
        if (!_canAttack && _currentCooldown <= 0)
        {
            _canAttack = true;
            _currentCooldown = 0;
        }
    }

    /*public override void Attack(IDamageable damageableEntity)
    {
        if (_canAttack)
        {
            //damageableEntity.TakeDamage(attackDamage); // Hacer da�o a la entidad Damageable
            _currentCooldown = cooldown; // Reset del cooldown
            _canAttack = false;
            Debug.Log("AtaqueRosa");
        }
    }*/

    public override void Die()
    {
        _hasDied = true;
        // TODO: Pool
    }

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
        if (_canAttack && currentTarget != null && PlaceManager.Instance.objetoSiendoArrastrado == false)
        {
            ShootProyectile();
            _currentCooldown = cooldown; // Reset del cooldown
            _canAttack = false;
            animator.SetBool("ataque", true);
        }
        if (!_canAttack || currentTarget == null)
        {
            animator.SetBool("ataque", false);
        }
    }

    protected void ShootProyectile()
    {
        //Debug.Log("G");
    }

    public override void TakeDamage(float damageAmount)
    {
        // Da�ar torre
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
