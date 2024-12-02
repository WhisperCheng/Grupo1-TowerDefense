using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemigoIA : LivingEntityAI, IAttacker, IDamageable, IAnimatable
{
    [Header("Vida")] // Vida
    public float health;
    private HealthBar _healthBar;

    
    [Header("Ataque")] //Ataque
    public float attackDamage;
    public float cooldown = 1f;

    // Variables
    [Header("Variables Enemigo IA")]
    public float actionRadio;
    //public bool showActionRadio;
    
    protected Vector3 destination;
    [Header("Animaciones")]
    public Animator animatorController;

    private float _currentHealth;
    private float _maxHealth;
    private float _currentCooldown = 0f;
    private bool attackMode = false;
    private bool canAttack = true;
    protected float _maxSpeed;
    int playerMask = 1 << 7;
    private Transform _nearestRival;

    void Start()
    {
        //agent = GetComponent<NavMeshAgent>();
        _currentHealth = health;
        _maxHealth = health;
        _healthBar = GetComponentInChildren<HealthBar>();
        _currentCooldown = cooldown;
    }

    public virtual void WhileWalking() {
        AnimateWalking();
        Vector3 oldDest = destination;
        OnSearchingObjetives();
        if (oldDest != destination) // Para asignarle solo una vez la localización en caso de que sea la misma
        {
            OnAssignDestination(destination);
        }
    }

    protected Transform NearestForestHearthPos(string tag)
    {
        Transform nearestForestHearthPos = null;
        float minorDistance = Mathf.Infinity;

        GameObject[] heartList = GameObject.FindGameObjectsWithTag(tag);

        // Se comprueba y elige al corazón con menor distancia
        if (heartList.Length > 0)
        {
            foreach (GameObject gameObj in heartList)
            {
                // Distancia entre el enemigo y el objetivo
                float actualDistance = Vector3.Distance(transform.position, gameObj.transform.position);
                if (actualDistance < minorDistance)
                {
                    /* Se detectan enemigos dentro del radio de acción pero hay que comprobar que
                     * no hay muros por delante*/
                    if (ThereAreNoObstacles(gameObj.transform))
                    {
                        minorDistance = actualDistance;
                        nearestForestHearthPos = gameObj.transform;
                    }
                }
            }
        }
        return nearestForestHearthPos; // puede llegar a ser nulo si no hay nada al rededor, hay que                    
    }                               // tenerlo en cuenta

    protected Transform NearestRival(Collider[] listaChoques)
    {
        // Esta lista almacenará el resultado de llamar a OverlapSphere
        //Collider[] listaChoques;

        //listaChoques = Physics.OverlapSphere(transform.position, radio, mascara);
        Transform enemigoMasCercano = null;
        float menorDistancia = Mathf.Infinity; // TODO

        // Se comprueba y elige el enemigo con menor distancia
        if (listaChoques.Length > 0)
        {
            foreach (Collider choque in listaChoques)
            {
                float distanciaActual = Vector3.Distance(transform.position, choque.transform.position);
                if (distanciaActual < menorDistancia)
                {
                    /* Se detectan enemigos dentro del radio de acción pero hay que comprobar que
                     * no hay muros por delante*/
                    if (ThereAreNoObstacles(choque.transform))
                    {
                        menorDistancia = distanciaActual;
                        enemigoMasCercano = choque.transform;
                    }
                }
            }
        }
        return enemigoMasCercano; // puede llegar a ser nulo si no hay nada al rededor, hay que                    
        // tenerlo en cuenta
    }

    public void OnSearchingObjetives()
    {
        // Esta lista almacenará el resultado de llamar a OverlapSphere y detectar al jugador
        Collider[] listaChoques;
        listaChoques = Physics.OverlapSphere(transform.position, actionRadio, playerMask);

        // Se obtiene al jugador más cercano
        Transform nearestRival = NearestRival(listaChoques);

        if (nearestRival != null) // Si detecta a un jugador en el radio de acción, se pondrá a perseguirle
        {                                                   // y atacarle
            destination = nearestRival.position;
            //OnAttack();
            //TODO
        }
        else // Si no hay un jugador dentro del radio de acción, pasa a ir hacia el corazón del bosque
        {
            //OnAbandonAtacking();
            //TODO
            Transform hearth = NearestForestHearthPos(GameManager.Instance.tagCorazonDelBosque);
            if (hearth != null)
                destination = hearth.position;
        }
    }
    

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, actionRadio);
        //Gizmos.DrawRay(transform.position, transform.forward);
    }
#endif

    public void OnAttack()
    {
        // Efectos de partículas al golpear, cambiar animación, etc
    }
    private void Attack(IDamageable damageableEntity)
    {
        if (canAttack)
        {
            Debug.Log("A");
            damageableEntity.TakeDamage(attackDamage); // Hacer daño a la entidad Damageable
            _currentCooldown = cooldown; // Reset del cooldown
            canAttack = false;
        }
    }

    protected void ManageCooldown()
    {
        _currentCooldown -= Time.deltaTime;
        if (!canAttack && _currentCooldown <= 0)
        {
            canAttack = true;
            _currentCooldown = 0;
        }
    }

    private void OnAbandonAtacking()
    {
        attackMode = false;
    }

    private void OnAssignDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public override void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        _maxSpeed = agent.speed;
    }

    public void Die()
    {
        Destroy(this.gameObject);
        // TODO: Devolver a la pool
    }

    public void TakeDamage(float damageAmount)
    {
        // Dañar enemigo
        _currentHealth -= damageAmount;

        // TODO ? Spawn particulas de golpe?

        // Actualizar barra de vida
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void AnimateWalking()
    {
        if (animatorController != null)
        {
            animatorController.SetFloat("Velocidad", (agent.velocity.magnitude / _maxSpeed));
        }
    }

    private void OnTriggerStay(Collider collision)
    {

        if (collision.tag == GameManager.Instance.tagCorazonDelBosque)
        {
            IDamageable hearthEntity = collision.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
            Attack(hearthEntity);
        }
    }
}
