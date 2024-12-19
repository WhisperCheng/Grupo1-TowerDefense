using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivoraTower : Tower
{
    [Header("Vida")] // Vida
    public float health;
    protected HealthBar _healthBar;

    [Header("Ataque")] //Ataque
    public float attackDamage;
    public float cooldown = 1f;

    //[Header("Animaciones")]
    protected Animator animator;

    [Header("Partículas de construcción")]
    [SerializeField] protected ParticleSystem particlesPlacing;

    protected List<Collider> attackingList;

    protected float _currentHealth;
    protected float _maxHealth;
    protected float _currentCooldown = 0f;
    protected float _maxSpeed;

    protected bool _attackMode = false;
    protected bool _canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_locked)
        {
            CheckAvailableRivals();
            EnemyDetection();
            LookRotation();
            ManageCombat();
        }
    }

    public override void Init()
    {
        base.Init();
        attackingList = new List<Collider>();
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
            //OnAttack();
        }
    }

    // Método necesario para que cuando no haya ningún enemigo dentro de la hitbox de ataque, automáticamente elimine a los
    // enemigos a atacar de la lista de targets
    protected void CheckAvailableRivals()
    {
        for (int i = 0; i < attackingList.Count; i++)
        {
            Collider col = attackingList[i];
            if (col == null || !col.gameObject.activeSelf || !col.enabled)
            {
                attackingList.Remove(col);
            }
        }
    }

    public override void OnAttack() // Al atacar se resetean los parámetros booleanos a su estado normal
    {
        if (_canAttack && _attackMode && !_locked && currentTarget != null)
        {
            _currentCooldown = cooldown; // Reset del cooldown
            //_canAttack = false;
        }
        /*else
        {
            _attackMode = false;
        }*/

        /*if (!_hasEnemyAssigned || currentTarget == null)
        {
            //animator.SetBool("AttackMode", false);
        }*/
    }

    public void Attack(IDamageable damageableEntity)
    {
        //animator.SetTrigger("Attack");
        animator.SetBool("AttackMode", true);
        _attackMode = true;
        if (_canAttack)
        {
            damageableEntity.TakeDamage(attackDamage); // Hacer daño a la entidad Damageable
            //_currentCooldown = cooldown; // Reset del cooldown
            //_canAttack = false;
        }
    }

    protected override void OnDamageTaken()
    {
        //
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

    public override void Die()
    {
        _hasDied = true;
        ReturnToPool();
    }

    public override float GetHealth()
    {
        return health;
    }
    

    public override void ReturnToPool()
    {
        if (_initialized)
        {
            _locked = true;
            _currentHealth = health; // Restaurar la salud del caballero al valor máximo
            _healthBar = GetComponentInChildren<HealthBar>();
            _healthBar.ResetHealthBar(); // Actualizamos la barra de salud
        }
        CarnivorousPlantPool.Instance.ReturnCarnivorousPlant(this.gameObject);
    }

    public override GameObject GetFromPool()
    {
        return CarnivorousPlantPool.Instance.GetCarnivorousPlant();
    }


    public override GameObject RestoreToDefault()
    {
        if (!_locked)
        {// Si ya ha sido enviado previamente a la pool, se resetean los valores por defecto
            Init();
            _locked = true;
            enabled = true;
            _attackMode = false;
            _canAttack = false;
            animator.SetBool("AttackMode", false); // Dejar de reproducir animación de atacar
        }
        return this.gameObject;
    }

    public void TowerAttackEvent()
    {
        if (currentTarget != null)
        {
            _canAttack = true;
        }
    }

    /*public override void UnlockTower()
    {
        base.UnlockTower();
        particlesPlacing.Play();
    }*/

    // Este método se encarga de atacar a todos los objetivos que estén dentro de la zona de ataque incluidos en el array de objetivos a atacar
    protected void ManageCombat()
    {
        if (_canAttack && _attackMode)
        { // Se recorre la lista de los objetivos a atacar y se les hace daño
            for (int i = 0; i < attackingList.Count; i++)
            {
                if (attackingList[i] != null && attackingList[i].enabled)
                {
                    IDamageable entity = attackingList[i].GetComponent<IDamageable>();
                    Attack(entity);
                }
            }
            _canAttack = false; // Se quita el modo de atacar
            _currentCooldown = cooldown; // Reset del cooldown
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        IDamageable entity = collision.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
        //if (collision.tag == GameManager.Instance.tagCorazonDelBosque)
        bool validEnemyCollision = collision.GetType().ToString().Equals("UnityEngine.BoxCollider") &&
            entity != null && collision.tag == "Enemy" && entity.GetHealth() > 0;
        if (validEnemyCollision)
        {
            if (attackingList.Contains(collision))
            {
                _attackMode = true;
                animator.SetTrigger("Attack");
            }
        }
    }


    private void OnTriggerEnter(Collider collision)
    {
        IDamageable entity = collision.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
        //if (collision.tag == GameManager.Instance.tagCorazonDelBosque)
        //bool isEnemy = collision.tag != "Ally" && collision.tag != "Tower"
        if (entity != null && collision.tag == "Enemy" && entity.GetHealth() > 0)
        {
            if (!attackingList.Contains(collision)) // Si la lista para almacenar rivales dentro de la hitbox de ataque
            {                                       // no contiene a la entidad, se almacena en ella
                attackingList.Add(collision);
                Debug.Log("añadido " + attackingList.Count + collision.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {

        IDamageable entity = collision.GetComponent(typeof(IDamageable)) as IDamageable;
        if (entity != null && collision.tag == "Enemy")
        {
            // Si se sale un rival de la hitbox de ataque, se elimina de la lista de enemigos dentro del área de ataque
            attackingList.Remove(collision);
        }
    }
}
