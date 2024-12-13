using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[SelectionBase]
public abstract class EnemyAI : LivingEntityAI, IDamageable, IPoolable
{
    protected NavMeshAgent _agent;
    // Variables
    [Header("Variables Enemigo IA")]
    public float actionRadio;
    public float checkWaypointProximityDistance = 3;
    public float _onFocusAcceleration = 30;

    [Header("Tags de obst�culos a ignorar")]
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

    [Header("Animaciones")]
    public Animator animatorController;

    protected Vector3 _destination;
    protected float _defaultAcceleration;
    protected float _currentHealth;
    protected float _maxHealth;
    protected float _currentCooldown = 0f;
    protected float _maxSpeed;

    protected List<Collider> attackingList;

    protected bool _attackMode = false;
    protected bool _canDamage = false;
    protected bool _finishedWaypoints = false;
    protected bool _initialized = false;
    
    protected Transform _nearestRival;
    protected int _currentWaypointIndex = 0;

    protected int _playerMask = 1 << 7;
    protected int _allyMask = 1 << 9;

    protected abstract void WhileWalking();
    public abstract void OnAttack(); // Efectos de part�culas al golpear, cambiar animaci�n, etc
    public abstract void Attack(IDamageable damageableEntity);
    public abstract void Die();
    protected abstract void OnDamageTaken(); // Efectos de part�culas y efectos visuales al recibir da�o
    public abstract float GetHealth();
    protected abstract void CheckIfRivalsInsideAttackRange();
    protected abstract void ReturnToPool();
    public abstract GameObject RestoreToDefault();

    // Invoca autom�ticamente la implementaci�n del m�todo abstracto Init() para las clases herederas
    void Start()
    {
        Init();
    }
    public override void Init()
    {
        _initialized = true;
        _currentHealth = health; // Inicializar/Restaurar la salud del caballero al valor m�ximo
        _maxHealth = health;
        _currentCooldown = cooldown;
        
        _healthBar = GetComponentInChildren<HealthBar>();
        _agent = GetComponent<NavMeshAgent>();
        _maxSpeed = _agent.speed;
        _currentWaypointIndex = 0;
        _destination = GameManager.Instance.wayPoints[_currentWaypointIndex].transform.position;
        OnAssignDestination(_destination);
        _currentCooldown = 0f;
        animatorController = GetComponent<Animator>();
        attackingList = new List<Collider>();
        _defaultAcceleration = _agent.acceleration;
    }

    protected void OnAssignDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }

    /// Si es necesario, las clases herederas podr�n usar estos m�todos para implementar variaciones de
    /// funcionalidad, y sobre todo usarlos como m�todos/herramientas ya definidas en esta clase base --- ///
    protected virtual void OnSearchingObjetives()
    {
        // Esta lista almacenar� el resultado de llamar a OverlapSphere y detectar al jugador
        Collider[] listaChoques;
        listaChoques = Physics.OverlapSphere(transform.position, actionRadio, (_playerMask | _allyMask));

        // Se obtiene al jugador m�s cercano
        Transform nearestRival = EntityUtils.NearestRivalOnNavMesh(listaChoques, transform.position, ignoreTagList, true, reachAttackRange);
        if (nearestRival != null)
            // Si detecta a un rival (vivo) en el radio de acci�n, se pondr� a perseguirle
        {                       // y atacarle
            _destination = nearestRival.position;

            _agent.acceleration = _onFocusAcceleration; // Cambiar la aceleraci�n de rotaci�n del enemigo
            var targetRotation = Quaternion.LookRotation(_destination - transform.position); // Rotar suavemente
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _defaultAcceleration / 2 * Time.deltaTime);
        }
        else // Si no hay un jugador dentro del radio de acci�n, pasa a ir hacia el waypoint correspondiente
        {
            _agent.acceleration = _defaultAcceleration; // Cambiar la aceleraci�n de rotaci�n del enemigo a la original
            if (_finishedWaypoints)
            { // Si ya ha recorrido todo los waypoints, ir hacia el coraz�n del bosque m�s cercano
                Transform hearth = EntityUtils.GetNearestForestHearthPos(transform.position, ignoreTagList);
                if (hearth != null && _destination != hearth.position) _destination = hearth.position;
                // Si el coraz�n existe y la posici�n es distinta, se actualiza el destino
            } else // Si no, va yendo de waypoint en waypoint hasta llegar al final
            {
                if (_attackMode)
                {
                    _attackMode = false;
                }
                
                Transform nearestWaypoint = EntityUtils.GetNearestWayPoint(transform.position);
                // Vuelve a tomar la ruta de los waypoints (waypoint actual)
                
                int nearestWaypointIndex = Array.FindIndex(GameManager.Instance.wayPoints, i => (nearestWaypoint.position == i.transform.position));

                // Si entra dentro del radio de acci�n para detectar el waypoint m�s cercano,
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
                    } // Vuelve a tomar la ruta con el nuevo destino actualizado si no s ha actualizado antes
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

    protected void ManageCooldown() // No
    {
        _currentCooldown -= Time.deltaTime;
        if (!_canDamage && _currentCooldown <= 0)
        {
            //_canDamage = true;
            _currentCooldown = 0;
        }
    }

    public void AttackEvent()
    {
        if (_attackMode) // Si est� �nicamente en modo de ataque (cuando hay un rival dentro de la hitbox de ataque,
        {               // activar el booleano de hacer da�o)
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
        // Da�ar enemigo
        _currentHealth -= damageAmount;
        OnDamageTaken();

        // Actualizar barra de vida
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    public virtual void AnimateWalking()
    {
        if (animatorController != null)
        {
            //animatorController.SetFloat("Velocidad", (agent.velocity.magnitude / _maxSpeed));
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
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
