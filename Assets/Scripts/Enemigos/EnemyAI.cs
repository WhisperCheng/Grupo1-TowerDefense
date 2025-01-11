using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[SelectionBase]
public abstract class EnemyAI : LivingEntityAI, IDamageable, IPoolable, IPoisonable
{
    protected float poisonedTime = 0;
    protected NavMeshAgent _agent;
    // Variables
    //public Transform hearth;
    [Header("Variables Enemigo IA")]
    public float actionRadio;
    public float checkWaypointProximityDistance = 3;
    public float _onFocusAcceleration = 30;
    public float speed = 3.5f;

    [Header("Tags de obstáculos a ignorar")]
    public string[] ignoreTagList;

    [Header("Vida")] // Vida
    public float health;
    protected HealthBar _healthBar;
    [SerializeField] protected ParticleSystem _particulasMuerte;
    [SerializeField] protected ParticleSystem _particulasGolpe;

    [Header("Ataque")] //Ataque
    public float attackDamage;
    public float cooldown;
    public float reachAttackRange = 3;

    //[Header("Animaciones")]
    protected Animator animatorController;

    protected Vector3 _destination;
    protected float _defaultAcceleration;
    protected float _currentHealth;
    protected float _maxHealth;
    protected float _currentCooldown = 0f;
    protected float _maxSpeed;
    protected float _originalSpeed;

    //protected List<Collider> attackingList;

    protected bool _attackMode = false;
    protected bool _canDamage = false;
    protected bool _finishedWaypoints = false;
    protected bool _initialized = false;
    
    protected Transform _nearestRival;
    protected int _currentWaypointIndex = 0;

    protected int _playerMask = 1 << 7;
    protected int _allyMask = 1 << 9;

    protected abstract void WhileWalking();
    public virtual void OnAttack() { } // Efectos de partículas al golpear, cambiar animación, etc
    protected virtual void OnDamageTaken() { } // Efectos de partículas y efectos visuales al recibir daño
    public abstract float GetHealth();
    public abstract void ReturnToPool();
    public abstract GameObject RestoreToDefault();
    protected abstract void ManageCombat();
    public abstract GameObject GetFromPool();

    // Invoca automáticamente la implementación del método abstracto Init() para las clases herederas
    void Start()
    {
        _originalSpeed = speed;
        Init();
    }
    public override void Init()
    {
        _initialized = true;
        _currentHealth = health; // Inicializar/Restaurar la salud del caballero al valor máximo
        _maxHealth = health;
        _currentCooldown = cooldown;
        
        _healthBar = GetComponentInChildren<HealthBar>();
        _agent = GetComponent<NavMeshAgent>();
        speed = _originalSpeed;
        _maxSpeed = _originalSpeed;
        _agent.speed = _originalSpeed;
        _currentWaypointIndex = 0;
        _destination = GameManager.Instance.wayPoints[_currentWaypointIndex].transform.position;
        OnAssignDestination(_destination);
        _currentCooldown = 0f;
        animatorController = GetComponent<Animator>();
        //attackingList = new List<Collider>();
        _defaultAcceleration = _agent.acceleration;
    }
    protected virtual void UpdateCurrentCooldown()
    {
        if (_currentCooldown > 0)
        {
            _currentCooldown -= Time.deltaTime;
        }
        if (_currentCooldown < 0)
        {
            _currentCooldown = 0;
        }
    }

    protected void OnAssignDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }

    protected void ManagePoisonCooldown()
    {
        if (poisonedTime > 0)
            poisonedTime -= Time.deltaTime;
    }

    public virtual void Die()
    {
        // Cada enemigo ejecuta su funcion de volver a la pool
        RoundManager.enemiesAlive--;
        ReturnToPool();
    }

    /// Si es necesario, las clases herederas podrán usar estos métodos para implementar variaciones de
    /// funcionalidad, y sobre todo usarlos como métodos/herramientas ya definidas en esta clase base --- ///
    protected virtual void OnSearchingObjetives()
    {
        // Esta lista almacenará el resultado de llamar a OverlapSphere y detectar al jugador
        Collider[] listaChoques;
        listaChoques = Physics.OverlapSphere(transform.position, actionRadio, (_playerMask | _allyMask));

        // Se obtiene al jugador más cercano
        Transform nearestRival = EntityUtils.NearestRivalOnNavMesh(_agent, listaChoques, transform.position, null, 
            true, reachAttackRange);
        if (nearestRival != null)
            // Si detecta a un rival (vivo) en el radio de acción, se pondrá a perseguirle
        {                       // y atacarle
            _destination = nearestRival.position;

            _agent.acceleration = _onFocusAcceleration; // Cambiar la aceleración de rotación del enemigo
            var targetRotation = Quaternion.LookRotation(_destination - transform.position); // Rotar suavemente
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _defaultAcceleration / 2 * Time.deltaTime);
        }
        else // Si no hay un jugador dentro del radio de acción, pasa a ir hacia el waypoint correspondiente
        {
            _agent.acceleration = _defaultAcceleration; // Cambiar la aceleración de rotación del enemigo a la original
            if (_finishedWaypoints)
            { // Si ya ha recorrido todo los waypoints, ir hacia el corazón del bosque más cercano
                Transform hearth = EntityUtils.GetNearestForestHearthPos(transform.position, null);
                if (hearth != null && _destination != hearth.position) _destination = hearth.position;
                // Si el corazón existe y la posición es distinta, se actualiza el destino
            } else // Si no, va yendo de waypoint en waypoint hasta llegar al final
            {
                if (_attackMode)
                {
                    _attackMode = false;
                }
                Transform nearestWaypoint = EntityUtils.GetNearestWayPoint(transform.position);
                // Vuelve a tomar la ruta de los waypoints (waypoint actual)
                
                int nearestWaypointIndex = Array.FindIndex(GameManager.Instance.wayPoints, i => (nearestWaypoint.position == i.transform.position));

                // Si entra dentro del radio de acción para detectar el waypoint más cercano,
                // cambiar el destino al siguiente
                if (nearestWaypoint != null)
                {
                    bool isNextWaypoint = nearestWaypoint.position == GameManager.Instance.wayPoints[nearestWaypointIndex].transform.position;
                    if (isNextWaypoint && Vector3.Distance(transform.position, nearestWaypoint.position) < checkWaypointProximityDistance)
                    {
                        //Debug.Log(_currentWaypointIndex + " - " + _destination);
                        UpdateWayPointDestination(++nearestWaypointIndex);
                    }
                }
                
                if (!_finishedWaypoints)
                {
                    Vector3 dest = GameManager.Instance.wayPoints[_currentWaypointIndex].transform.position;
                    if (dest != _destination)
                    {
                        _destination = dest;
                    } // Vuelve a tomar la ruta con el nuevo destino actualizado si no se ha actualizado antes
                }
            }
        }
    }

    protected void UpdateWayPointDestination(int index)
    {
        _currentWaypointIndex = index;
        //_destination = waypoint.position;
        // Hasta que no llegue al final se actualizan los waypoints, pero si llega al final dejan de actualizarse
        // y se activa el booleano finishedWaypoints
        if (_currentWaypointIndex >= GameManager.Instance.wayPoints.Length)
        {
            _finishedWaypoints = true;
        } else
        {
            _finishedWaypoints = false;
        }
    }

    /*protected void ManageCooldown() // No
    {
        _currentCooldown -= Time.deltaTime;
        if (!_canDamage && _currentCooldown <= 0)
        {
            //_canDamage = true;
            _currentCooldown = 0;
        }
    }*/

    public void AttackEvent()
    {
        if (_attackMode) // Si está únicamente en modo de ataque (cuando hay un rival dentro de la hitbox de ataque,
        {               // activar el booleano de hacer daño)
            _canDamage = true;
        }
        else
        {
            _canDamage = false;
        }
    }

    public void CheckAttackModeEvent()
    {
        if (!_attackMode)
        {
            _attackMode = false; // Modo de ataque = falso, se usa para terminar de atacar
            _canDamage = false;
        }
    }

    public virtual void TakeDamage(float damageAmount) // Se puede sobreescribir (virtual), por si es necesario
    {
        // Dañar enemigo si está habilitado
        if (isActiveAndEnabled)
        {
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
    public virtual void AnimateWalking()
    {
        _agent.speed = speed;
        if (animatorController != null)
        {
            //animatorController.SetFloat("Velocidad", (agent.velocity.magnitude / _maxSpeed));
        }
    }

    public void PoisonEntity(float time, float poisonInterval, float poisonDamage)
    {
        StopAllCoroutines(); // Reset de las corutinas de envenenamiento
        if (gameObject.activeSelf)
        {
            ColorUtils.ChangeObjectMaterialColors(gameObject, GameManager.Instance.materialPropertyVeneno);
            StartCoroutine(PoisonCoroutine(time, poisonInterval, poisonDamage));
        }
    }

    private IEnumerator PoisonCoroutine(float time, float poisonInterval, float poisonDamage) // Corutina para hacer daño cada x tiempo a causa del efecto de veneno
    {
        poisonedTime = time;
        while (gameObject.activeSelf && poisonedTime > 0) // While loop para hacer daño cada x tiempo
        {
            TakeDamage(poisonDamage);
            yield return new WaitForSeconds(poisonInterval);
            if (poisonedTime < 0)
                poisonedTime = 0;
        }
        ColorUtils.ChangeObjectMaterialColors(gameObject, null); // Volver a aplicar el color normal una vez se termine la duración del veneno
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, actionRadio);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, reachAttackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkWaypointProximityDistance);
        //Gizmos.DrawRay(transform.position, transform.forward);
    }
#endif
}
