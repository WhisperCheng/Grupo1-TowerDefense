using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedTower : Tower
{
    [Header("Vida")] // Vida
    public float health;
    protected HealthBar _healthBar;
    [SerializeField] protected ParticleSystem _particulasMuerte;
    [SerializeField] protected ParticleSystem _particulasGolpe;

    [Header("Ataque")] //Ataque
    public float attackDamage;
    public float cooldown = 1f;

    [Header("Animaciones")]
    protected Animator animator;

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
    private void Update()
    {
        EnemyDetection();
        if (PlaceManager.Instance.objetoSiendoArrastrado == false)
        {
            LookRotation();
        }
    }

    public override void Attack(IDamageable damageableEntity)
    {
        throw new System.NotImplementedException();
    }

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
    }

    public override void OnAttack()
    {
        if (currentTarget != null && PlaceManager.Instance.objetoSiendoArrastrado == false)
        {
            ShootProyectile();
            animator.SetBool("ataque", true);
        }
        if (currentTarget == null)
        {
            animator.SetBool("ataque", false);
        }
    }

    protected void ShootProyectile()
    {
        Debug.Log("G");
    }

    public override void TakeDamage(float damageAmount)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnDamageTaken()
    {
        throw new System.NotImplementedException();
    }

    public override bool HasDied()
    {
        return _hasDied;
    }
}
