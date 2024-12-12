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
        // TODO: Devolver a la pool
    }

    public override float GetHealth()
    {
        return _currentHealth;
    }
    protected override void OnDamageTaken()
    {
        //Debug.Log("809");
    }

    public void ResetValues()
    {
        _currentHealth = health;
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth); // Actualizamos la barra de salud
        _destination = GameManager.Instance.wayPoints[_currentWaypointIndex].transform.position;
    }

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
