using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedTower : LivingTower, IDamageable, IBoosteable
{
    [Header("Vida")] // Vida
    public float health;
    protected HealthBar _healthBar;
    [SerializeField] protected ParticleSystem _particulasMuerte;
    [SerializeField] protected ParticleSystem _particulasGolpe;

    protected Animator animator;

    [Header("Proyectil")]
    [SerializeField] protected Transform shooterSource;
    [Range(0, 100)]
    [SerializeField] protected float shootingSpeed;

    protected float _currentHealth;
    protected float _maxHealth;
    protected float _maxSpeed;

    protected bool _attackMode = false;
    protected bool _canAttack = true;

    protected override void Start()
    {
        base.Start();
        base.Init();
        Init();
    }

    protected void Update()
    {
        if (!locked)
        {
            EnemyDetection();
            LookRotation();
            UpdateCurrentCooldown();
        }
    }

    protected override void UpdateCurrentCooldown()
    {
        base.UpdateCurrentCooldown();
        animator.SetFloat("Cooldown", _currentCooldown);
    }

    public override void ReturnToPool()
    {
        if (_initialized && _loaded)
        {
            locked = true;
            _currentHealth = health; // Restaurar la salud del caballero al valor m�ximo
            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar.ResetHealthBar(); // Actualizamos la barra de salud
            _hasEnemyAssigned = false;
            _attackMode = false;
            currentTarget = null;
            money = _originalMoney;
            _boostIndex = -1;
            cooldown = _originalCooldown;
            RemoveExistentCrowns();
            // Importante: usar el metodo base. para luego hacer override en cada instancia y a�adir el retorno a la pool
        }
    }

    public override GameObject RestoreToDefault()
    {
        if (!locked)
        {// Si ya ha sido enviado previamente a la pool, se resetean los valores por defecto
            Init();
            _canAttack = false;
            _hasEnemyAssigned = false;
            _attackMode = false;
            currentTarget = null;
            money = _originalMoney;
        }
        return this.gameObject;
    }

    protected override void EnemyDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, _enemyMask);

        if (colliders.Length > 0)
        {
            bool insideRange = false;
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject != null && collider.gameObject.activeSelf
                    && collider.gameObject == currentTarget) // Se comprueba si el enemigo est� contenido dentro de la nueva lista de colisiones
                {                               // De ser as�, se actualiza insideRange a true y se deja de buscar dentro del bucle de colliders
                    insideRange = true;
                    break;
                }
            }
            Collider lastEnemy = colliders[colliders.Length - 1]; // Escoge al �ltimo enemigo que entr�
            if (!_hasEnemyAssigned) // Si no tiene ning�n enemigo asignado, se le asigna el enemigo
            {
                currentTarget = lastEnemy.gameObject;
                _hasEnemyAssigned = true;
                _attackMode = true;
            }
            else
            { // Si tiene un enemigo asignado pero este es desactivado o enviado a la pool o pasa a estar fuera de rango, entonces
                if (!insideRange) // se descarta como objetivo para pasar posteriormente a buscar uno nuevo que s� est� dentro de rango
                {
                    currentTarget = null;
                    _hasEnemyAssigned = false;
                    _attackMode = false;
                }
            }
        }
        else // Si no se han detectado enemigos, el target actual es nulo y no le hace focus a nada
        {
            if (currentTarget != null)
            {
                currentTarget = null;
            }
            _hasEnemyAssigned = false;
            _attackMode = false;
        }

        if (_hasEnemyAssigned) // Si tiene un enemigo asignado que es� dentro del rango, empieza a atacar
        {
            _attackMode = true;
            _canAttack = true;
            OnAttack();
        }

        animator.SetBool("AttackMode", _attackMode);
    }

    public override void Init()
    {
        base.Init();
        animator = GetComponent<Animator>();
        _currentHealth = health;
        _maxHealth = health;
        _currentCooldown = cooldown;
        _healthBar = GetComponentInChildren<HealthBar>();
        _enemyMask = 1 << GameManager.Instance.layerEnemigos;
        _attackMode = false;
        animator.SetBool("AttackMode", _attackMode); // Dejar de reproducir animaci�n de atacar
    }

    public void OnAttack()
    {
        if (_canAttack && _attackMode && _currentCooldown <= 0 && !locked && currentTarget != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    public virtual void ShootProyectileEvent()
    {
        if (currentTarget != null)
        {
            _attackMode = false;
            animator.SetBool("AttackMode", false);
            _currentCooldown = cooldown; // Reset del cooldown
        }
        // Cada RangedTower implementa su l�gica de disparo
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
    public override float GetHealth() { return _currentHealth; }
    public override float GetMaxHealth() { return _maxHealth; }
    
}
