using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public abstract class BasicEnemyAI : EnemyAI
{
    public AttackBox attackBox;

    void Update()
    {
        WhileWalking();
        ManagePoisonCooldown();
        UpdateCurrentCooldown();
        ManageCombat();
    }

    // Este método se encarga de atacar a todos los objetivos que estén dentro de la zona de ataque incluidos en el array de objetivos a atacar
    protected override void ManageCombat()
    {

        // int attackMasks = 1 << GameManager.Instance.layerJugador | 1 << GameManager.Instance.layerAliados 
        //| 1 << GameManager.Instance.layerCorazon;
        int attackMasks = 0;
        foreach (int layerNum in enemyToAttackLayers)
        {
            if (layerNum != GameManager.Instance.layerCorazon)
                attackMasks |= 1 << layerNum;
        }

        bool attackDone = attackBox.ManageAttack(gameObject.transform, attackMasks | 1 << GameManager.Instance.layerCorazon,
            animatorController, _canDamage, attackDamage);
        _attackMode = attackBox.AttackModeBool; // Se actualizan los booleanos que manejan el combate al valor correspondiente
        _canDamage = attackBox.CanAttackOrDamageBool; // actualizado por el attackBox
        if (attackDone)
        { // Si se ataca con éxito a los enemigos dentro del área de ataque, se resetea el cooldown
            if (attackBox.AttackedEntity.CompareTag(GameManager.Instance.tagCorazonDelBosque)) { _dropMoney = false; Die(); }
            // Si se ataca a un corazón del bosque, el enemigo se muere
            _currentCooldown = cooldown;
        }
    }

    protected override void WhileWalking()
    {
        animatorController.SetBool("AttackMode", _attackMode);
        AnimateWalking();
        OnSearchingObjetives();
        OnAssignDestination(_destination);
    }

    public override void Die()
    {
        GameObject deathParticles = EnemyDeathParticlesPool.Instance.GetEnemyDeathParticles();
        deathParticles.transform.position = transform.position;
        deathParticles.GetComponent<ParticleSystem>().Play();
        // SONIDO : Muerte
        base.Die();
    }

    public override float GetHealth() { return _currentHealth; }
    public override float GetMaxHealth() { return _maxHealth; }

    protected override void UpdateCurrentCooldown()
    {
        base.UpdateCurrentCooldown();
        animatorController.SetFloat("Cooldown", _currentCooldown);
    }

    public override void ReturnToPool()
    {
        _agent.updatePosition = false;
        _agent.Warp(GameManager.Instance.respawnEnemigos.position); // Se teleporta al respawn
        _agent.updatePosition = true;
        StopAllCoroutines(); // Reset de las corutinas de envenenamiento, si hay alguna activa
        _currentHealth = health; // Restaurar la salud del caballero al valor máximo
        _healthBar = GetComponentInChildren<HealthBar>();
        _healthBar.ResetHealthBar(); // Actualizamos la barra de salud
        ColorUtils.ChangeObjectMaterialColors(gameObject, null); // Volver a aplicar el color normal si ha sido envenenado

        ReturnEnemyGameObjectToPool();
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

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.red;
        attackBox.DrawGizmos(transform);
    }
#endif
}
