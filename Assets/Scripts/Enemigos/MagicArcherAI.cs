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

        _attackMode = true;
        if (_aimTarget != null)
        {
            GameObject currentTarget = _aimTarget.gameObject;
            Collider collider = currentTarget.GetComponent<Collider>();
            // El enemigo tiene que llevar un collider para que pueda obtener adecuadamente el centro del altura del enemigo
            CharacterController characterC = currentTarget.GetComponent<CharacterController>();
            float offsetYTargetPosition = 0;
            if (collider)
            {
                offsetYTargetPosition = collider.bounds.max.y / 2;
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
        animatorController.SetBool("AttackMode", false);
    }

    private Collider GetClosestEnemyCollider(Vector3 originPos, Collider[] enemyColliders)
    {
        float bestDistance = 99999.0f;
        Collider nearestCollider = null;

        foreach (Collider enemy in enemyColliders)
        {
            float distance = Vector3.Distance(originPos, enemy.transform.position);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                nearestCollider = enemy;
            }
        }

        return nearestCollider;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(shooterSource.transform.position, _destination + Vector3.up * 1.5f);
        Gizmos.DrawLine(shooterSource.transform.position, _destination + Vector3.up * 1.5f);
        Gizmos.DrawLine(predictive, predictive + Vector3.up * 10.5f);
    }
}
