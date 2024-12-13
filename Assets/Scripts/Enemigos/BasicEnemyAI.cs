using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyAI : EnemyAI
{

    // Update is called once per frame
    void Update()
    {
        WhileWalking();
        CheckIfRivalsInsideAttackRange();
        //ManageCooldown();
    }

    // Método necesario para que cuando no haya ningún enemigo dentro de la hitbox de ataque, automáticamente deje de poder atacar siempre
    // y de poder hacer daño.
    // Hay veces en las que el jugador al pasar por delante del enemigo y salirse justo antes de que termine la animación
    // de ataque que ejecuta "_canDamage = true" (animation event), al ejecutarse este código después de haber salido del hitbox de ataque
    // el OnTriggerExit no puede ejecutar "_canDamage = false", por lo que es necesario una lista que almacena los enemigos actuales
    // en rango y con ello determinar si puede seguir atacando o no.
    protected override void CheckIfRivalsInsideAttackRange()
    {
        if (_canDamage)
        {
            for (int i = 0; i < attackingList.Count; i++)
            {
                Collider col = attackingList[i];
                if (col == null || !col.enabled)
                {
                    attackingList.Remove(col);
                }
            }

            // Si la lista es tamaño == 0, desactivar el daño
            if (attackingList.Count == 0)
            {
                _attackMode = false;
                animatorController.SetBool("AttackMode", false);
                _canDamage = false;
            }
        }
        
    }

    protected override void WhileWalking()
    {
        animatorController.SetBool("AttackMode", _attackMode);
        AnimateWalking();
        Vector3 oldDest = _destination;
        OnSearchingObjetives();
        if (oldDest != _destination) // Para comprobar que el destino sea distinto y no estar todo el rato
        {                           // asignando la misma variable
            OnAssignDestination(_destination);
        }
    }

    public override void OnAttack()
    {
        // TODO: Efectos visuales al atacar
    }
    public override void Attack(IDamageable damageableEntity)
    {
        animatorController.SetTrigger("Attack");
        animatorController.SetBool("AttackMode", true);
        _attackMode = true;
        if (_canDamage)
        {
            damageableEntity.TakeDamage(attackDamage); // Hacer daño a la entidad Damageable
            _currentCooldown = cooldown; // Reset del cooldown
            _canDamage = false;
        }
    }

    public override void Die()
    {
        _particulasMuerte.Play();
        // Devolver a la pool
        ReturnToPool();
    }

    public override float GetHealth()
    {
        return _currentHealth;
    }
    protected override void OnDamageTaken()
    {
        //Debug.Log("809");
    }

    /*protected void ResetValues()
    {
        
    }*/

    private void OnTriggerStay(Collider collision)
    {
        IDamageable entity = collision.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
        //if (collision.tag == GameManager.Instance.tagCorazonDelBosque)
        if (entity != null && collision.tag != "Enemy" && entity.GetHealth() > 0)
        {
            Attack(entity); // Atacar a la entidad
            if (!attackingList.Contains(collision)) // Si la lista para almacenar rivales dentro de la hitbox de ataque
            {                                       // no contiene a la entidad, se almacena en ella
                attackingList.Add(collision);
            }
        }
        else
        {
            _canDamage = false;
        }
    }
    protected override void ReturnToPool()
    {
        // Desactivamos el NavMeshAgent y la IA del enemigo

        _agent.updatePosition = false;
        //_agent.updatePosition = false; // Desactivar el update de la IA para que no se ralle luego al hacerle tp y volverlo a activar
        //_agent.Warp(GameManager.Instance.respawnEnemigos.position); // Se teleporta al respawn
        transform.position = GameManager.Instance.respawnEnemigos.position;
        _agent.updatePosition = true;
        //_agent.enabled = false; // Se desactiva la IA
        //enabled = false; // Se desactiva el script para que no consuma recursos

        _currentHealth = health; // Restaurar la salud del caballero al valor máximo
        _healthBar = GetComponentInChildren<HealthBar>();
        _healthBar.ResetHealthBar(); // Actualizamos la barra de salud

        // Llamamos a la pool para devolver al caballero
        MiniKnightPool.Instance.ReturnMiniKnight(this.gameObject);
    }

    public override GameObject RestoreToDefault()
    {
        //if (GetComponent<NavMeshAgent>() != null)
        if (_initialized)
        {// Si ya ha sido enviado previamente a la pool, se resetean los valores por defecto
            Init();
            enabled = true;
            _attackMode = false;
            _canDamage = false;
            _finishedWaypoints = false;
            animatorController.SetBool("AttackMode", false); // Dejar de reproducir animación de atacar*/
        }
        return this.gameObject;
    }

    private void OnTriggerExit(Collider collision)
    {
        
        IDamageable entity = collision.GetComponent(typeof(IDamageable)) as IDamageable;
        if (entity != null && collision.tag != "Enemy")
        {
            animatorController.SetBool("AttackMode", false);
            if (attackingList.Contains(collision)) // Si se sale un rival de la hitbox de ataque, se elimina de
            {                                       // la lista de enemigos dentro del área de ataque
                attackingList.Remove(collision);
            }
            _canDamage = false;
        }
    }
}
