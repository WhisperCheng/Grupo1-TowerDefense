using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivoraTower : Tower, IDamageable
{
    [Header("Vida")] // Vida
    public float health;
    protected HealthBar _healthBar;

    [Header("Ataque")] //Ataque
    public float attackDamage;

    //[Header("Animaciones")]
    protected Animator animator;

    [Header("Partículas de construcción")]
    [SerializeField] protected ParticleSystem particlesPlacing;

    protected List<Collider> attackingList;

    protected float _currentHealth;
    protected float _maxHealth;
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
            UpdateCurrentCooldown();
            CheckAvailableRivals();
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

    public override void Init()
    {
        base.Init();
        if (attackingList != null)
        {
            attackingList.Clear();
        }
        else
        {
            attackingList = new List<Collider>();
        }
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
        //currentTargets.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, _enemyMask);

        //if (!_hasEnemyAssigned) currentTarget = null;
        if (colliders.Length > 0) // Si hay enemigos
        {
            bool insideRange = false;
            foreach (Collider collider in colliders)
            { 
                if(collider.gameObject != null && collider.gameObject.activeSelf 
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
        
        /*if(Vector3.Distance(gameObject.transform.position, currentTarget.transform.position) > range)
        {
            currentTarget = null;
            _hasEnemyAssigned = false;
        }*/

        if (_hasEnemyAssigned) // Si tiene un enemigo asignado que esé dentro del rango, empieza a atacar
        {
            _attackMode = true;
            //OnAttack();
        }
        //if (!_canAttack)
            //animator.ResetTrigger("Attack");

        animator.SetBool("AttackMode", _attackMode);
    }

    // Método necesario para que cuando no haya ningún enemigo dentro de la hitbox de ataque, automáticamente elimine a los
    // enemigos a atacar de la lista de targets
    protected void CheckAvailableRivals()
    {
        for (int i = 0; i < attackingList.Count; i++)
        {
            Collider col = attackingList[i];
            if (col == null || !col.gameObject.activeInHierarchy || !col.enabled ||
                Vector3.Distance(gameObject.transform.position, col.transform.position) > range)
            {
                attackingList.Remove(col);
                if (currentTarget == col)
                {
                    currentTarget = null;
                    _hasEnemyAssigned = false; // Para hacer "trigger" del bool y volver a elegir un nuevo enemigo
                }
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
        _attackMode = true;
        if (_canAttack)
        {
            damageableEntity.TakeDamage(attackDamage); // Hacer daño a la entidad Damageable
            //_currentCooldown = cooldown; // Reset del cooldown
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
        ReturnToPool();
    }

    public override float GetHealth()
    {
        return health;
    }
    

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
            //enabled = true;
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
            else
            { // Si la lista para almacenar rivales dentro de la hitbox de ataque no contiene a la entidad, se almacena en ella
                attackingList.Add(collision);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {

        IDamageable entity = collision.GetComponent(typeof(IDamageable)) as IDamageable;
        if (entity != null && collision.CompareTag("Enemy"))
        {
            // Si se sale un rival de la hitbox de ataque, se elimina de la lista de enemigos dentro del área de ataque
            attackingList.Remove(collision);
        }
    }
}
