using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MagicArcherAI : EnemyAI
{
    [Header("Variables Arquero")]
    public Transform shooterSource;
    public float shootingSpeed;
    public EvilProyectilFabric evilProyectilFabric;

    private Vector3 predictive = Vector3.zero;

    public override GameObject GetFromPool() { return ArcherPool.Instance.GetArcher(); }

    public override float GetHealth() { return _currentHealth; }
    public override float GetMaxHealth() { return _maxHealth; }

    // Update is called once per frame
    void Update()
    {
        WhileWalking();
        ManagePoisonCooldown();
        ManageCombat();
    }

    protected override void UpdateCurrentCooldown()
    {
        base.UpdateCurrentCooldown();
        animatorController.SetFloat("Cooldown", _currentCooldown);
    }

    public override GameObject RestoreToDefault()
    {
        // Si ya ha sido enviado previamente a la pool, se resetean los valores por defecto
        if (_initialized)
        {
            Init();
            enabled = true;
            _attackMode = false;
            _canDamage = false;
            _finishedWaypoints = false;
            animatorController.SetBool("AttackMode", false); // Dejar de reproducir animación de atacar
        }
        return this.gameObject;
    }

    public override void ReturnToPool()
    {
        _agent.speed = _maxSpeed;
        _agent.updatePosition = false;
        _agent.Warp(GameManager.Instance.respawnEnemigos.position); // Se teleporta al respawn
        _agent.updatePosition = true;
        StopAllCoroutines(); // Reset de las corutinas de envenenamiento, si hay alguna activa
        _currentHealth = health; // Restaurar la salud del caballero al valor máximo
        _healthBar = GetComponentInChildren<HealthBar>();
        _healthBar.ResetHealthBar(); // Actualizamos la barra de salud
        ColorUtils.ChangeObjectMaterialColors(gameObject, null); // Volver a aplicar el color normal si ha sido envenenado

        ArcherPool.Instance.ReturnArcher(gameObject);
    }
    protected override void ReturnEnemyGameObjectToPool() { ReturnToPool(); }

    protected override void ManageCombat() { UpdateCurrentCooldown(); }

    protected override void WhileWalking()
    {
        animatorController.SetBool("AttackMode", _attackMode);
        AnimateWalking();
        OnSearchingObjetives();
        OnAssignDestination(_destination);
    }

    protected override void OnSearchingObjetives()
    {
        base.OnSearchingObjetives();

        if (_attackMode) // Si está en modo de ataque / detecta a un enemigo se para
        {
            _agent.speed = 0;
        }
        if (!_attackMode && _agent.speed != 0) // Si deja de estar en modo de ataque restaura su velocidad
        {
            _agent.speed = _maxSpeed;
        }
    }


    public override void OnAttack()
    {
        //FMOD
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.roseShoot, this.gameObject);
        emitter.Play();

        int attackMasks = 0; // Reconocer a qué enemigos/layers hay que atacar (player, aliados)
        foreach (int layerNum in enemyToAttackLayers)
        {
            if (layerNum != GameManager.Instance.layerCorazon)
                attackMasks |= 1 << layerNum;
        }

        // Detección del enemigo dado el vector de su posición
        bool enemyDetected = Physics.SphereCast(_destination, 0.1f, Vector3.up, out RaycastHit hit, 100f, attackMasks);

        if (enemyDetected)
        {
            GameObject currentTarget = hit.transform.gameObject;
            Collider collider = hit.transform.gameObject.GetComponent<Collider>();
            NavMeshAgent agent = hit.transform.gameObject.GetComponent<NavMeshAgent>();
            CharacterController characterC = hit.transform.gameObject.GetComponent<CharacterController>();
            float offsetYTargetPosition = 0;
            if (collider)
            {
                offsetYTargetPosition = collider.bounds.max.y / 2;
            }
            if (agent)
            {
                offsetYTargetPosition = _agent.height / 2;
            }
            if (characterC)
            {
                offsetYTargetPosition = characterC.height / 2;
            }
            Vector3 offsetY = Vector3.up * offsetYTargetPosition;
            Vector3 predictivePosition =                           // Trayectoria predictiva
                ProyectileUtils.ShootingInterception
                .CalculateInterceptionPoint(shootingSpeed, shooterSource.transform, currentTarget.transform, offsetY);
            predictive = predictivePosition;
            evilProyectilFabric.LanzarEvilMagicProyectil(predictivePosition, shootingSpeed);
            _currentCooldown = cooldown;
        }
        _attackMode = false;
        animatorController.SetBool("AttackMode", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(shooterSource.transform.position, _destination + Vector3.up * 1.5f);
        Gizmos.DrawLine(shooterSource.transform.position, _destination + Vector3.up * 1.5f);
        Gizmos.DrawLine(predictive, predictive + Vector3.up * 10.5f);
    }
}
