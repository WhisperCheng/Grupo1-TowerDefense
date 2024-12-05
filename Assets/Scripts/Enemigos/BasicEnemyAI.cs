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
        ManageCooldown();
    }

    protected override void WhileWalking()
    {
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
        if (_canAttack)
        {
            damageableEntity.TakeDamage(attackDamage); // Hacer daño a la entidad Damageable
            _currentCooldown = cooldown; // Reset del cooldown
            _canAttack = false;
        }
    }

    public override void Die()
    {
        _particulasMuerte.Play();
        _hasDied = true;
        // TODO: Devolver a la pool
    }

    public override bool HasDied()
    {
        return _hasDied;
    }
    protected override void OnDamageTaken()
    {
        //Debug.Log("809");
    }

    private void OnTriggerStay(Collider collision)
    {

        if (collision.tag == GameManager.Instance.tagCorazonDelBosque)
        {
            IDamageable hearthEntity = collision.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
            Attack(hearthEntity);
        }
    }
}
