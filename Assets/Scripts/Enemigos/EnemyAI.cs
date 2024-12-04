using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[SelectionBase]
public abstract class EnemyAI : LivingEntityAI, IDamageable, IAnimatable
{
    protected NavMeshAgent agent;
    // Variables
    [Header("Variables Enemigo IA")]
    public float actionRadio;
    public float checkWaypointProximityDistance = 3;

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
    
    [Header("Animaciones")]
    public Animator animatorController;

    protected Vector3 _destination;

    protected float _currentHealth;
    protected float _maxHealth;
    protected float _currentCooldown = 0f;
    protected float _maxSpeed;

    protected bool _attackMode = false;
    protected bool _canAttack = true;
    protected bool _finishedWaypoints = false;
    
    protected Transform _nearestRival;
    protected int _currentWaypointIndex = 0;

    protected int _playerMask = 1 << 7;

    protected abstract void WhileWalking();
    public abstract void OnAttack(); // Efectos de partículas al golpear, cambiar animación, etc
    public abstract void Attack(IDamageable damageableEntity);
    public abstract void Die();
    protected abstract void OnDamageTaken(); // Efectos de partículas y efectos visuales al recibir daño

    /// --- Funciones y "Herramientas" base de las funciones principales y para las clases herederas --- ///
    /// Si es necesario, las clases herederas podrán usar estos métodos para implementar variaciones de
    /// funcionalidad, y sobre todo usarlos como métodos/herramientas ya definidas en esta clase base --- ///

    protected Transform GetNearestForestHearthPos(string tag)
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

    protected bool ThereAreNoObstacles(Transform nearestObjetive)
    {
        bool noObstacles = true;
        if (nearestObjetive != null)
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position, nearestObjetive.position, out hit))
            {/*
                if (hit.transform.tag != "Proyectil" && hit.collider.gameObject.tag != "Enemigo"
                    && hit.transform.tag != "Gnomo")
                {
                    noObstacles = false;
                }*/
                foreach (string name in ignoreTagList)
                {
                    if (hit.transform.tag != name)
                    {
                        noObstacles = false;
                        break;
                    }
                }
            }
        }

        return noObstacles;
    }

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

    protected void OnSearchingObjetives()
    {
        // Esta lista almacenará el resultado de llamar a OverlapSphere y detectar al jugador
        Collider[] listaChoques;
        listaChoques = Physics.OverlapSphere(transform.position, actionRadio, _playerMask);

        // Se obtiene al jugador más cercano
        Transform nearestRival = NearestRival(listaChoques);

        if (nearestRival != null) // Si detecta a un jugador en el radio de acción, se pondrá a perseguirle
        {                       // y atacarle
            _destination = nearestRival.position;
        }
        else // Si no hay un jugador dentro del radio de acción, pasa a ir hacia el waypoint correspondiente
        {
            if (_finishedWaypoints)
            { // Si ya ha recorrido todo los waypoints, ir hacia el corazón del bosque más cercano
                Transform hearth = GetNearestForestHearthPos(GameManager.Instance.tagCorazonDelBosque);
                if (hearth != null && _destination != hearth.position) _destination = hearth.position;
                // Si el corazón existe y la posición es distinta, se actualiza el destino
            } else // Si no, va yendo de waypoint en waypoint hasta llegar al final
            {
                Vector3 dest = GameManager.Instance.wayPoints[_currentWaypointIndex].position;
                if (dest != _destination)
                {
                    _destination = dest;
                }
                // Vuelve a tomar la ruta de los waypoints (waypoint actual)

                // Si entra dentro del radio de acción para detectar el siguiente waypoint,
                // cambiar el destino al siguiente
                if (Vector3.Distance(transform.position, _destination) < checkWaypointProximityDistance)
                {
                    UpdateWayPointDestination();
                }
            }
        }
    }

    protected void UpdateWayPointDestination()
    {
        _currentWaypointIndex++;
        // Hasta que no llegue al final se actualizan los waypoints, pero si llega al final dejan de actualizarse
        // y se activa el booleano finishedWaypoints
        if (_currentWaypointIndex >= GameManager.Instance.wayPoints.Length)
        {
            _finishedWaypoints = true;
        }
    }

    protected void ManageCooldown()
    {
        _currentCooldown -= Time.deltaTime;
        if (!_canAttack && _currentCooldown <= 0)
        {
            _canAttack = true;
            _currentCooldown = 0;
        }
    }
    public virtual void TakeDamage(float damageAmount) // Se puede sobreescribir (virtual), por si es necesario
    {
        // Dañar enemigo
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
            animatorController.SetFloat("Velocidad", (agent.velocity.magnitude / _maxSpeed));
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

    /*protected void OnAbandonAtacking()
    {
        _attackMode = false;
    }*/
}
