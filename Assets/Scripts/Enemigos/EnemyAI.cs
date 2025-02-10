using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[SelectionBase]

[RequireComponent(typeof(StudioEventEmitter))]

public abstract class EnemyAI : LivingEntityAI, IDamageable, IPoolable, IPoisonable
{
    //FMOD
    protected StudioEventEmitter emitter;

    protected float poisonedTime = 0;
    protected NavMeshAgent _agent;
    // Variables
    //public Transform hearth;
    [Header("Variables Enemigo IA")]
    public int dropMoney;
    public float actionRadio;
    public float checkWaypointProximityDistance = 3;
    public float _onFocusAcceleration = 30;
    public float _onFocusStoppingDistance = 0.39f;
    public float speed = 3.5f;
    public List<int> enemyToAttackLayers;
    public bool hasRangedAttack;

    [Header("Tags de obstáculos a ignorar")]
    public string[] ignoreTagList;

    [Header("Vida")] // Vida
    public float health;
    protected HealthBar _healthBar;

    [Header("Ataque")] //Ataque
    public float attackDamage;
    public float cooldown;
    public float reachAttackRange = 3;

    //[Header("Animaciones")]
    protected Animator animatorController;

    protected Vector3 _destination;
    protected Transform _aimTarget;
    protected float _defaultAcceleration;
    protected float _defaultOnStoppingDistance;
    protected float _currentHealth;
    protected float _maxHealth;
    protected float _currentCooldown = 0f;
    protected float _maxSpeed;
    public float MaxSpeed { get { return _maxSpeed; } }

    protected bool _attackMode = false;
    protected bool _canDamage = false;
    protected bool _dropMoney = true;
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
    public abstract float GetMaxHealth();
    public abstract void ReturnToPool();
    public abstract GameObject RestoreToDefault();
    protected abstract void ManageCombat();
    public abstract GameObject GetFromPool();

    protected abstract void ReturnEnemyGameObjectToPool();

    // Invoca automáticamente la implementación del método abstracto Init() para las clases herederas
    void Start()
    {
        _maxSpeed = speed;
        Init();

        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.hitmarker, this.gameObject);

    }
    public override void Init()
    {
        _initialized = true;
        _currentHealth = health; // Inicializar/Restaurar la salud del enemigo al valor máximo
        _maxHealth = health;
        _currentCooldown = cooldown;
        _dropMoney = true;
        _healthBar = GetComponentInChildren<HealthBar>();
        _agent = GetComponent<NavMeshAgent>();
        speed = _maxSpeed;
        _agent.speed = _maxSpeed;
        _currentWaypointIndex = 0;
        _destination = GameManager.Instance.wayPoints[_currentWaypointIndex].transform.position;
        OnAssignDestination(_destination);
        _currentCooldown = 0f;
        animatorController = GetComponent<Animator>();
        _defaultAcceleration = _agent.acceleration;
        _defaultOnStoppingDistance = _agent.stoppingDistance;
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
        if (RoundManager.enemiesAlive > 0)
            RoundManager.enemiesAlive--;
        ReturnToPool();
        if (_dropMoney)
            MoneyManager.Instance.AddMoney(dropMoney);
    }

    /// Si es necesario, las clases herederas podrán usar estos métodos para implementar variaciones de
    /// funcionalidad, y sobre todo usarlos como métodos/herramientas ya definidas en esta clase base --- ///
    protected virtual void OnSearchingObjetives()
    {
        // Esta lista almacenará el resultado de llamar a OverlapSphere y detectar al jugador
        Collider[] listaChoques;

        int attackMasks = 0;
        foreach (int layerNum in enemyToAttackLayers)
        {
            if (layerNum != GameManager.Instance.layerCorazon)
                attackMasks |= 1 << layerNum;
        }

        listaChoques = Physics.OverlapSphere(transform.position, actionRadio, attackMasks);

        // Se obtiene al jugador más cercano
        Transform nearestRival = EntityUtils.NearestRivalOnNavMesh(_agent, listaChoques, transform.position, null,
            true, reachAttackRange);
        if (nearestRival != null)
        // Si detecta a un rival (vivo) en el radio de acción, se pondrá a perseguirle
        {                       // y atacarle
            _destination = nearestRival.position;

            _agent.acceleration = _onFocusAcceleration; // Cambiar la aceleración de rotación del enemigo
            _agent.stoppingDistance = _onFocusStoppingDistance; // Cambiar la stoppingDistance del enemigo
            var targetRotation = Quaternion.LookRotation(_destination - transform.position); // Rotar suavemente
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _defaultAcceleration / 2 * Time.deltaTime);
            if (hasRangedAttack)
            { // Solo se pone en modo de animación de ataque si el enemigo tiene ataque a distancia activado
                _attackMode = true;
                _aimTarget = nearestRival;
            }
        }
        else // Si no hay un jugador dentro del radio de acción, pasa a ir hacia el waypoint correspondiente
        {
            _agent.acceleration = _defaultAcceleration; // Cambiar la aceleración de rotación del enemigo a la original
            _agent.stoppingDistance = _defaultOnStoppingDistance; // Cambiar la stoppingDistance del enemigo a la original
            if (_finishedWaypoints)
            { // Si ya ha recorrido todo los waypoints, ir hacia el corazón del bosque más cercano
                Transform hearth = EntityUtils.GetNearestForestHearthPos(transform.position, null);
                if (hearth != null && _destination != hearth.position)
                {
                    _destination = hearth.position;
                    if(hasRangedAttack)
                        _aimTarget = hearth;
                }
                // Si el corazón existe y la posición es distinta, se actualiza el destino

                if (hasRangedAttack) // Si el enemigo tiene disparo de proyectiles, empieza el modo de ataque de proyectiles
                    _attackMode = true;
            }
            else // Si no, va yendo de waypoint en waypoint hasta llegar al final
            {
                _aimTarget = null;
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
        // Hasta que no llegue al final se actualizan los waypoints, pero si llega al final dejan de actualizarse
        // y se activa el booleano finishedWaypoints
        if (_currentWaypointIndex >= GameManager.Instance.wayPoints.Length)
        {
            
            _finishedWaypoints = true;
        }
        else
        {
            _finishedWaypoints = false;
        }
    }

    public void AttackEvent()
    {
        if (_attackMode) // Si está únicamente en modo de ataque (cuando hay un rival dentro de la hitbox de ataque,
        {               // activar el booleano de hacer daño)
            _canDamage = true;
            OnAttack();
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

            //FMOD
            emitter.Play();


            // Actualizar barra de vida
            _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
            if (_currentHealth <= 0)
            {
                emitter.Play();
                Die();
            }
        }
    }
    public virtual void AnimateWalking()
    {
        _agent.speed = speed;
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
    }
#endif
}
