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

            if (!_attackMode && _agent.speed != 0) // Si deja de estar en modo de ataque restaura su velocidad
            {
                _agent.speed = _maxSpeed;
            }
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

        // Para apuntar hacia el centro del aliado (detectar el linecast ya que hace uso de una posición y no del gameobject)
        Vector3 direction = _destination - gameObject.transform.position;
        direction = direction.normalized;
        bool enemyDetected = Physics.Linecast(shooterSource.transform.position, _destination + Vector3.up * 1.5f,
            out RaycastHit hitInfo, attackMasks);

        if (enemyDetected)
        {
            GameObject currentTarget = hitInfo.transform.gameObject;
            Collider collider = hitInfo.transform.gameObject.GetComponent<Collider>();
            NavMeshAgent agent = hitInfo.transform.gameObject.GetComponent<NavMeshAgent>();
            CharacterController characterC = hitInfo.transform.gameObject.GetComponent<CharacterController>();
            float offsetYTargetPosition = 0;
            if (collider)
            {
                offsetYTargetPosition = collider.bounds.max.y;
            }
            if (agent)
            {
                offsetYTargetPosition = _agent.height / 2;
            }
            if (characterC)
            {
                offsetYTargetPosition = characterC.velocity.magnitude;
            }
            Vector3 offsetY = Vector3.up * offsetYTargetPosition;
            Vector3 predictivePosition =                           // Trayectoria predictiva
                ProyectileUtils.ShootingInterception
                .CalculateInterceptionPoint(shootingSpeed, shooterSource.transform, currentTarget.transform, offsetY);
            evilProyectilFabric.LanzarEvilMagicProyectil(predictivePosition, shootingSpeed);
            _currentCooldown = cooldown;
        }
        _attackMode = false;
        animatorController.SetBool("AttackMode", false);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(shooterSource.transform.position, _destination + Vector3.up * 1.5f);
    }
}
