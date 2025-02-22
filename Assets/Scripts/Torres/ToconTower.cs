using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ToconBrain))]
public class ToconTower : StaticTower
{
    [Header("Spawn Setas Aliadas")]
    [SerializeField] private Transform spawn;

    [Header("Cantidad Setas Aliadas")]
    [SerializeField] private int maximaCantidadSetas;

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
    protected override void OnDamageTaken() { }
    public override void Init()
    {
        base.Init();
        brain = GetComponent<ToconBrain>();
        currentTarget = null;
        _hasEnemyAssigned = false;
    }
    protected void EnemyDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, _enemyMask);

        if (colliders.Length > 0)
        {
            bool insideRange = false;
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject != null && collider.gameObject.activeSelf
                    && collider.gameObject == currentTarget) // Se comprueba si el enemigo est� contenido dentro de la nueva lista de colisiones
                {                               // De ser as�, se actualiza insideRange a true y se deja de buscar dentro del bucle de colliders
                    insideRange = true;
                    break;
                }
            }
            Collider lastEnemy = colliders[colliders.Length - 1]; // Escoge al �ltimo enemigo que entr�
            if (!_hasEnemyAssigned) // Si no tiene ning�n enemigo asignado, se le asigna el enemigo
            {
                currentTarget = lastEnemy.gameObject;
                _hasEnemyAssigned = true;
            }
            else
            { // Si tiene un enemigo asignado pero este es desactivado o enviado a la pool o pasa a estar fuera de rango, entonces
                if (!insideRange) // se descarta como objetivo para pasar posteriormente a buscar uno nuevo que s� est� dentro de rango
                {
                    currentTarget = null;
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
        }

        if (currentTarget == null) // Si no tiene un enemigo asignado entonces las setas se ir�n a su casa
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

    public override GameObject GetFromPool() { return AllyTowerPool.Instance.GetAllyTower(); }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);

    }
}
