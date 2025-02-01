using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]


public class CarnivoraTower : LivingTower, IDamageable
{

    //FMOD
    private StudioEventEmitter emitter;

    [Header("Vida")] // Vida
    public float health;
    protected HealthBar _healthBar;

    [Header("Ataque")] //Ataque
    public float attackDamage;
    private float _originalAttackDamage;
    public AttackBox attackBox;
    public Transform attackBoxHolder;

    //[Header("Animaciones")]
    protected Animator animator;

    [Header("Partículas de construcción")]
    [SerializeField] protected ParticleSystem particlesPlacing;

    protected float _currentHealth;
    protected float _maxHealth;
    protected float _maxSpeed;

    protected bool _attackMode = false;
    protected bool _canAttack = true;
    
    protected override void Start()
    {
        base.Start();
        Init();

        //FMOD
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.carnivorousShoot, this.gameObject);

    }

    void Update()
    {
        if (!locked)
        {
            UpdateCurrentCooldown();
            EnemyDetection();
            LookRotation();
            ManageCombat();
        }
    }
    protected override void UpdateCurrentCooldown()
    {
        base.UpdateCurrentCooldown();
        animator.SetFloat("Cooldown", _currentCooldown);
    }

    public override void Boost()
    {
        if (NextBoostMoney() != -1) // Mientras hayan boosts disponibles
        {
            attackDamage += boostPrices[_boostIndex + 1].damageAddition;
        }
        base.Boost();
    }
    public override void Init()
    {
        base.Init();
        _originalAttackDamage = attackDamage;
        _hasEnemyAssigned = false;
        animator = GetComponent<Animator>();
        _currentHealth = health;
        _maxHealth = health;
        _currentCooldown = cooldown;
        currentTarget = null;
        _healthBar = GetComponentInChildren<HealthBar>();
        _enemyMask = 1 << GameManager.Instance.layerEnemigos;
    }

    protected override void EnemyDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, _enemyMask);

        if (colliders.Length > 0) // Si hay enemigos, procesarlos
        {
            bool insideRange = false;
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject != null && collider.gameObject.activeSelf
                    && collider.gameObject == currentTarget) // Se comprueba si el enemigo está contenido dentro de la nueva lista de colisiones
                {                               // De ser así, se actualiza insideRange a true y se deja de buscar dentro del bucle de colliders
                    insideRange = true;
                    break;
                }
            }
            foreach (Collider collider in colliders)
            {
                if (!_hasEnemyAssigned && collider.gameObject != null && collider.gameObject.activeSelf)
                { // Si no tiene ningún enemigo asignado, se le asigna uno
                    currentTarget = collider.gameObject;
                    _hasEnemyAssigned = true;
                    _attackMode = true;
                    break;
                }
                else
                { // Si tiene un enemigo asignado pero este es desactivado o enviado a la pool o pasa a estar fuera de rango, entonces
                    if (!insideRange) // se descarta como objetivo para pasar posteriormente a buscar uno nuevo que sí esté dentro de rango
                    {
                        currentTarget = null;
                        _hasEnemyAssigned = false;
                        _attackMode = false;
                    }
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

        if (_hasEnemyAssigned) // Si tiene un enemigo asignado que esé dentro del rango, empieza a atacar
        {
            _attackMode = true;
        }
        if (!_canAttack)
        animator.ResetTrigger("Attack");

        animator.SetBool("AttackMode", _attackMode);
    }
    public void OnAttack() // Al atacar se resetean los parámetros booleanos a su estado normal
    {
        if (_canAttack && _attackMode && !locked && currentTarget != null)
        {
            _currentCooldown = cooldown; // Reset del cooldown
        }
    }
    public override void TakeDamage(float damageAmount)
    {
        if (gameObject.activeSelf)
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
    }
    public override void Die() { ReturnToPool(); }
    public override float GetHealth() { return health; }
    public override float GetMaxHealth() { return _maxHealth; }
    public override void ReturnToPool()
    {
        if (_initialized && _loaded)
        {
            locked = true;
            attackDamage = _originalAttackDamage;
            _currentHealth = health; // Restaurar la salud del caballero al valor máximo
            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar.ResetHealthBar(); // Actualizamos la barra de salud
            _hasEnemyAssigned = false;
            _attackMode = false;
            currentTarget = null;
            money = _originalMoney;
            _boostIndex = -1;
            RemoveExistentCrowns();
        }
        CarnivorousPlantPool.Instance.ReturnCarnivorousPlant(this.gameObject);
    }

    public override GameObject GetFromPool() { return CarnivorousPlantPool.Instance.GetCarnivorousPlant(); }
    public override GameObject RestoreToDefault()
    {
        if (!locked)
        {// Si ya ha sido enviado previamente a la pool, se resetean los valores por defecto
            Init();
            locked = true;
            //enabled = true;
            _attackMode = false;
            _canAttack = false;
            animator.SetBool("AttackMode", false); // Dejar de reproducir animación de atacar
            money = _originalMoney;
        }
        return this.gameObject;
    }
    public void TowerAttackEvent() { if (currentTarget != null) _canAttack = true; emitter.Play(); }

  
    // Este método se encarga de atacar a todos los objetivos que estén dentro de la zona de ataque incluidos en el array de objetivos a atacar
    protected void ManageCombat()
    {
        int attackMasks = 1 << GameManager.Instance.layerEnemigos;

        bool attackDone = attackBox.ManageAttack(attackBoxHolder, gameObject.transform, attackMasks, animator, _canAttack, attackDamage);
        _attackMode = attackBox.AttackModeBool; // Se actualizan los booleanos que manejan el combate al valor correspondiente
        _canAttack = attackBox.CanAttackOrDamageBool; // actualizado por el attackBox
        if (attackDone)
        { // Si se ataca con éxito a los enemigos dentro del área de ataque, se resetea el cooldown
            _currentCooldown = cooldown;
        }


    }
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
        attackBox.DrawGizmos(attackBoxHolder);
    }
}
