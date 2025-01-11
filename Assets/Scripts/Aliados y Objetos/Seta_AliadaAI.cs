using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Seta_AliadaAI : LivingEntityAI, IDamageable, IPoolable
{
    //VARIABLES
    [Header("Variables Seta Aliada IA")]
    //public int radio = 15;
    public float speed = 4f;
    public float cooldown = 0f;
    public float distanceFromWhichCanGetInsideHome = 1f;
    //private int longRayo = 1;

    [Header("Vida")] // Vida
    public float vida = 10;
    private HealthBar _healthBar;

    [Header("Ataque")] //Ataque
    public float attackDamage = 1;
    //public float cooldown;
    public float reachAttackRange = 3;
    public AttackBox attackBox;


    //GAMEOBJECTs
    private ToconBrain toconBrain;
    public ToconBrain ToconBrain { private get; set; } // Encapsulación del toconBrain con getter y setter
    private GameObject enemigoActual; //maza o el que sea que sigue la seta
    //private List<Collider> objetivosDeAtaqueActuales = new List<Collider>();

    //CONTROLADORES
    private Animator _animator;
    private Rigidbody rb;
    private NavMeshAgent _navAgent;
    private RaycastHit raycast;

    private float _currentHealth;
    private float _maxHealth;
    private float _maxVelocity;
    private float _currentCooldown = 0f;

    private bool _initialized = false;
    private bool _canDamage = false;
    private bool _attackMode = false;

    private int _velocityHash;

    public override void Init()
    {
        _initialized = true;
        _currentHealth = vida; // Inicializar/Restaurar la salud del caballero al valor máximo
        _maxHealth = vida;
        _healthBar = GetComponentInChildren<HealthBar>();
        _navAgent = GetComponent<NavMeshAgent>();
        _currentCooldown = 0f;
        _animator = GetComponent<Animator>();
        //objetivosDeAtaqueActuales = new List<Collider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _velocityHash = Animator.StringToHash("VelMagnitud");
        //
        //animator = GetComponent<Animator>();
        //navAgent = GetComponent<NavMeshAgent>();
        Init();
        _maxVelocity = _navAgent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveAndEnabled)
        {
            UpdateCurrentCooldown();
            SeguirEnemigo();
            AnimarSeta();
            //CheckRivalsInsideAttackRange();
            ManejarAtaqueAEnemigos();
        }
        
        //AtacarEnemigo();


        //Si no tiene enemigo seleccionado
        //Buscar enemigo dentro del rango
        //Si tiene enemigo lockeado
        //Ir hacia el
        //Si estas ya enfrente de el 
        //Atacarlo
            // si lo atacas y no esta enemigo lockeado null 

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
        _animator.SetFloat("Cooldown", _currentCooldown);
    }

    private void AnimarSeta()
    {
        _animator.SetFloat(_velocityHash, _navAgent.velocity.magnitude / _maxVelocity);
    }

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

    private bool ComprobarSiEsNuevoDestino(Vector3 dest)
    {
        return _navAgent.destination != dest;
    }

    private void SeguirEnemigo()
    {
        enemigoActual = toconBrain.ObjetivoActual;

        if (enemigoActual)
        {
            Vector3 enemigoPos = toconBrain.ObjetivoActual.transform.position;
            if (ComprobarSiEsNuevoDestino(enemigoPos)) // Si tiene una nueva posición diferente, se actualiza
            {
                _navAgent.SetDestination(enemigoPos);
            }
            //_animator.SetBool("Caminar", true);
            _attackMode = true;
        }
        else
        { // Cuando no haya enemigos a detectar, se van todos a su casa y se van a la pool de objetos
            if (ComprobarSiEsNuevoDestino(toconBrain.HomePos))
            _navAgent.SetDestination(toconBrain.HomePos);
            _attackMode = false;

            if(Vector3.Distance(transform.position, toconBrain.HomePos) <= distanceFromWhichCanGetInsideHome)
            {
                Die(); // Retornar a la pool y descontar aliado del toconBrain
            }
        }
        _animator.SetBool("AttackMode", _attackMode);
    }
    public void SetToconBrain(ToconBrain brain)
    {
        toconBrain = brain;
    }
    private void ManejarAtaqueAEnemigos()
    {
        /*if (_canDamage)
        {
            foreach (Collider col in objetivosDeAtaqueActuales)
            {
                IDamageable enemigo = col.GetComponent<IDamageable>();
                enemigo.TakeDamage(attackDamage); 
            }
        }*/

        /*if (_canDamage && _attackMode)
        { // Se recorre la lista de los objetivos a atacar y se les hace daño
            for (int i = 0; i < objetivosDeAtaqueActuales.Count; i++)
            {
                if (objetivosDeAtaqueActuales[i] != null && objetivosDeAtaqueActuales[i].enabled && objetivosDeAtaqueActuales[i].gameObject.activeSelf)
                {
                    IDamageable entity = objetivosDeAtaqueActuales[i].GetComponent<IDamageable>();
                    Attack(entity); // Atacar a la entidad Damageable
                }
            }
            _animator.ResetTrigger("Attack");
            //_attackMode = false;
            _canDamage = false; // Se quita el modo de atacar
            _currentCooldown = cooldown; // Reset del cooldown
        }*/
        int attackMasks = 1 << GameManager.Instance.layerEnemigos;

        bool attackDone = attackBox.ManageAttack(gameObject.transform, attackMasks, _animator, _canDamage, attackDamage);
        _attackMode = attackBox.AttackModeBool; // Se actualizan los booleanos que manejan el combate al valor correspondiente
        _canDamage = attackBox.CanAttackOrDamageBool; // actualizado por el attackBox
        if (attackDone)
        { // Si se ataca con éxito a los enemigos dentro del área de ataque, se resetea el cooldown
            _currentCooldown = cooldown;
        }
    }

    /*private void CheckRivalsInsideAttackRange()
    {
        for (int i = 0; i < objetivosDeAtaqueActuales.Count; i++)
        {
            Collider col = objetivosDeAtaqueActuales[i];
            if (col == null || !col.gameObject.activeSelf)
            {
                objetivosDeAtaqueActuales.Remove(col);
            }
        }

        if (_canDamage)
        {
            // Si la lista es tamaño == 0, desactivar el daño
            if (objetivosDeAtaqueActuales.Count == 0)
            {
                _attackMode = false;
                _animator.SetBool("AttackMode", false);
                _canDamage = false;
            }
        }
    }*/

    public void Die()
    {
        toconBrain.QuitarAliado();
        ReturnToPool();
    }

    public void TakeDamage(float damageAmount)
    {
        if (isActiveAndEnabled)
        {
            _currentHealth -= damageAmount;
            //OnDamageTaken();

            // Actualizar barra de vida
            _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
            if (_currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public float GetHealth()
    {
        return vida;
    }

    public GameObject RestoreToDefault()
    {
        //if (GetComponent<NavMeshAgent>() != null)
        if (_initialized)
        {// Si ya ha sido enviado previamente a la pool, se resetean los valores por defecto
            Init();
            _attackMode = false;
            _canDamage = false;
            _animator.SetBool("AttackMode", false); // Dejar de reproducir animación de atacar
        }
        return this.gameObject;
    }

    public GameObject GetFromPool()
    {
        return AllyPool.Instance.GetAlly();
    }

    public void ReturnToPool()
    {
        _currentHealth = vida; // Restaurar la salud del aliado al valor máximo
        _healthBar = GetComponentInChildren<HealthBar>();
        _healthBar.ResetHealthBar(); // Actualizamos la barra de salud

        // Llamamos a la pool para devolver al aliado
        AllyPool.Instance.ReturnAlly(this.gameObject);
    }

    /*private void OnTriggerStay(Collider collision)
    {
        IDamageable entity = collision.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
        //if (collision.tag == GameManager.Instance.tagCorazonDelBosque)
        if (entity != null && collision.tag == GameManager.Instance.tagEnemigos && entity.GetHealth() > 0)
        {
            if (objetivosDeAtaqueActuales.Contains(collision))
            {
                _attackMode = true;
                _animator.SetTrigger("Attack");
                
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        IDamageable entity = collision.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
        if (entity != null && collision.tag == GameManager.Instance.tagEnemigos && entity.GetHealth() > 0)
        {
            if (!objetivosDeAtaqueActuales.Contains(collision)) // Si la lista para almacenar rivales dentro de la hitbox de ataque
            {                                       // no contiene a la entidad, se almacena en ella
                objetivosDeAtaqueActuales.Add(collision);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {

        IDamageable entity = collision.GetComponent(typeof(IDamageable)) as IDamageable;
        if (entity != null && collision.tag == GameManager.Instance.tagEnemigos)
        {
            _animator.SetBool("AttackMode", false);
            // Si se sale un rival de la hitbox de ataque, se elimina de la lista de enemigos dentro del área de ataque
            objetivosDeAtaqueActuales.Remove(collision);
        }
    }*/

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, reachAttackRange);
        attackBox.DrawGizmos(transform);
    }
#endif
}
