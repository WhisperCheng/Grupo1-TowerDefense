using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ToconBrain))]
public class ToconTower : Tower
{
    [Header("Spawn Setas Aliadas")]
    [SerializeField] private Transform spawn;

    [Header("Cantidad Setas Aliadas")]
    [SerializeField] private int maximaCantidadSetas;

    //[Header("Cooldown de spawn")]
    //[SerializeField] private float spawnCooldown;

    private ToconBrain brain;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!locked)
        {
            brain.SpawnCooldown = cooldown; // Actualizar constantemente variables del cerebro si es necesario
            brain.MaxNumAliados = maximaCantidadSetas;
            EnemyDetection();
        }
    }

    public override void Die() { } // El tocón no tiene vida, pero los enemigos sí, por lo que nunca va a morir
    public override void TakeDamage(float damageAmount) { } // Lo mismo con TakeDamage y OnDamageTaken y otras funciones
    protected override void OnDamageTaken() { }
    public override float GetHealth() { return 0; }
    public override void OnAttack() { }
    public override void Init()
    {
        base.Init();
        brain = GetComponent<ToconBrain>();
        currentTarget = null;
        _hasEnemyAssigned = false;
    }
    protected override void EnemyDetection()
    {
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
                currentTarget = lastEnemy.gameObject;
                _hasEnemyAssigned = true;
                //_attackMode = true;
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
            //_attackMode = false;
        }
        
        if (currentTarget == null) // Si no tiene un enemigo asignado entonces las setas se irán a su casa
        {
            _hasEnemyAssigned = false;
        }

        brain.ObjetivoActual = currentTarget;
    }
    public override void UnlockTower()
    {
        base.UnlockTower();
        brain = GetComponent<ToconBrain>();
        brain.ResetValues(spawn.transform.position, cooldown, maximaCantidadSetas); // Reset cerebro e iniciar a partir del momento del desbloqueo el spawn de aliados
        brain.ActivarSpawn();
        //brain.SpawnCooldown = spawnCooldown;
    }

    public override void ReturnToPool()
    {
        if (_initialized)
        {
            locked = true;
        }
        AllyTowerPool.Instance.ReturnAllyTower(this.gameObject);
    }

    public override GameObject RestoreToDefault()
    {
        // TODO
        if (_initialized)
        {
            Init();
        }
        
        return gameObject;
    }

    public override GameObject GetFromPool()
    {
        return AllyTowerPool.Instance.GetAllyTower();
    }
}
