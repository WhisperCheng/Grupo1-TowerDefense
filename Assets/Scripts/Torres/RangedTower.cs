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
    public float projectileDamage;
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
        base.Init();
        Init();
    }

    protected void Update()
    {
        if (!_locked)
        {
            //ManageCooldown(); // TODO: Implementar cooldown
            EnemyDetection();
            //LookRotation();
        }
    }

    protected void LateUpdate()
    {
        if (!_locked)
        {
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
        if (_initialized && _loaded)
        {
            _locked = true;
            _currentHealth = health; // Restaurar la salud del caballero al valor máximo
            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar.ResetHealthBar(); // Actualizamos la barra de salud
            _hasEnemyAssigned = false;
            _attackMode = false;
            currentTarget = null;
            //animator.keepAnimatorStateOnDisable = false;
            
            // Importante: usar el metodo base. para luego hacer override en cada instancia y añadir el retorno a la pool
        }
    }

    public override GameObject RestoreToDefault()
    {
        if (!_locked)
        {// Si ya ha sido enviado previamente a la pool, se resetean los valores por defecto
            Init();
            _canAttack = false;
            _hasEnemyAssigned = false;
            _attackMode = false;
            currentTarget = null;
            /*animator.enabled = true;
            animator.Rebind();
            animator.Update(0f);*/
            /* animator.keepAnimatorStateOnDisable = true;*/
            /*animator.Rebind();
            animator.Update(0f);
            animator.Play("Idle", 0);*/
            //Debug.Log("a " + );
        }
        return this.gameObject;
    }

    /*public override void SetLoaded(bool loaded)
    {
        base.SetLoaded(loaded);
        if (animator != null)
        {
            animator.enabled = true;
            animator.Rebind();
            animator.Update(0f);
        }
    }*/

    protected override void EnemyDetection()
    {
        //currentTargets.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, _enemyMask);
       
        if (colliders.Length > 0)
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
            Collider lastEnemy = colliders[colliders.Length - 1]; // Escoge al último enemigo que entró
            if (!_hasEnemyAssigned) // Si no tiene ningún enemigo asignado, se le asigna el enemigo
            {
                //currentTargets.Add(collider.gameObject);
                currentTarget = lastEnemy.gameObject;
                _hasEnemyAssigned = true;
                _attackMode = true;
            }
            else
            { // Si tiene un enemigo asignado pero este es desactivado o enviado a la pool o pasa a estar fuera de rango, entonces
                if (!insideRange) // se descarta como objetivo para pasar posteriormente a buscar uno nuevo que sí esté dentro de rango
                {
                    currentTarget = null;
                    //_hasEnemyAssigned = false;
                    //_attackMode = false;
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
            OnAttack();
        }
        
        animator.SetBool("AttackMode", _attackMode);
    }

    public override void Init()
    {
        animator = GetComponent<Animator>();
        _currentHealth = health;
        _maxHealth = health;
        _currentCooldown = cooldown;
        _healthBar = GetComponentInChildren<HealthBar>();
        _enemyMask = 1 << GameManager.Instance.layerEnemigos;
        _attackMode = false;
        animator.SetBool("AttackMode", _attackMode); // Dejar de reproducir animación de atacar
    }

    public override void OnAttack()
    {
        if (_canAttack && _attackMode && !_locked && currentTarget != null)
        {
            _currentCooldown = cooldown; // Reset del cooldown
            animator.SetTrigger("Attack");
        }
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
