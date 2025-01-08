using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyAI : EnemyAI
{
    public Vector3 attackingBoxSize;
    public Vector3 attackingBoxPos;
    // Update is called once per frame
    void Update()
    {
        WhileWalking();
        ManagePoisonCooldown();
        UpdateCurrentCooldown();
        //CheckRivalsInsideAttackRange();
        ManageCombat();
    }

    // Método necesario para que cuando no haya ningún enemigo dentro de la hitbox de ataque, automáticamente deje de poder atacar siempre
    // y de poder hacer daño.
    // Hay veces en las que el jugador al pasar por delante del enemigo y salirse justo antes de que termine la animación
    // de ataque que ejecuta "_canDamage = true" (animation event), al ejecutarse este código después de haber salido del hitbox de ataque
    // el OnTriggerExit no puede ejecutar "_canDamage = false", por lo que es necesario una lista que almacena los enemigos actuales
    // en rango y con ello determinar si puede seguir atacando o no.
    protected override void CheckRivalsInsideAttackRange()
    {
        /*for (int i = 0; i < attackingList.Count; i++)
        {
            Collider col = attackingList[i];
            if (col == null || !col.gameObject.activeSelf)
            {
                attackingList.Remove(col);
            }
        }

        if (_canDamage)
        {
            // Si la lista es tamaño == 0, desactivar el daño
            if (attackingList.Count == 0)
            {
                _attackMode = false;
                animatorController.SetBool("AttackMode", false);
                _canDamage = false;
            }
        }*/

    }
    // Este método se encarga de atacar a todos los objetivos que estén dentro de la zona de ataque incluidos en el array de objetivos a atacar
    protected override void ManageCombat()
    {
        /*if (_canDamage && _attackMode)
        { // Se recorre la lista de los objetivos a atacar y se les hace daño
            for (int i = 0; i < attackingList.Count; i++)
            {
                if (attackingList[i] != null && attackingList[i].enabled && attackingList[i].gameObject.activeSelf)
                {
                    IDamageable entity = attackingList[i].GetComponent<IDamageable>();
                    Attack(entity);
                }
            }
            _currentCooldown = cooldown; // Reset del cooldown
            _attackMode = false;
            _canDamage = false; // Se quita el modo de atacar
            _currentCooldown = cooldown; // Reset del cooldown
        }*/


        Vector3 center = transform.position + attackingBoxPos;
        //transform.TransformPoint(attackingBoxSize);
        Collider[] allies = Physics.OverlapBox(center, attackingBoxSize, Quaternion.identity, 1 << GameManager.Instance.layerJugador);

        if (allies.Length > 0)
        {
            if (_canDamage)
            {
                foreach (Collider col in allies) // En este bucle solo va a estar el jugador, así que solo se va a ejecutar una vez
                {
                    if (col != null && col.gameObject.activeInHierarchy) // Por si acaso el jugador no está activo 
                    {                                                       // (no debería de ocurrir en teoría)
                        _attackMode = true;
                        animatorController.SetTrigger("Attack");

                        IDamageable damageableEntity = col.gameObject.GetComponent<IDamageable>();
                        if (damageableEntity != null && damageableEntity.GetHealth() > 0)
                        {
                            Attack(damageableEntity);
                        }
                    }
                }
                _canDamage = false;
                _attackMode = false; // Reset del bool para hacer daño y del de modo de ataque
            }
        }
        else
        {
            _attackMode = false;
            animatorController.ResetTrigger("Attack");
        }
    }

    protected override void WhileWalking()
    {
        animatorController.SetBool("AttackMode", _attackMode);
        AnimateWalking();
        Vector3 oldDest = _destination;
        OnSearchingObjetives();
        //if (oldDest != _destination) // Para comprobar que el destino sea distinto y no estar todo el rato
        //{                           // asignando la misma variable
            OnAssignDestination(_destination);
        //}
    }

    public override void OnAttack()
    {
        // TODO: Efectos visuales al atacar
    }
    public override void Attack(IDamageable damageableEntity)
    {
        if (_canDamage)
        {
            damageableEntity.TakeDamage(attackDamage); // Hacer daño a la entidad Damageable
            
        }
    }

    public override void Die()
    {
        //_particulasMuerte.Play();
        base.Die();
    }

    public override float GetHealth()
    {
        return _currentHealth;
    }
    protected override void OnDamageTaken()
    {
        //Debug.Log("809");
    }

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

        // Llamamos a la pool para devolver al caballero
        MiniKnightPool.Instance.ReturnMiniKnight(this.gameObject);
    }

    public override GameObject GetFromPool()
    {
        return MiniKnightPool.Instance.GetMiniKnight();
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
            animatorController.SetBool("AttackMode", false); // Dejar de reproducir animación de atacar
        }
        return this.gameObject;
    }

    /*private void OnTriggerStay(Collider collision)
    {
        IDamageable entity = collision.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
        //if (collision.tag == GameManager.Instance.tagCorazonDelBosque)
        if (entity != null && collision.tag != "Enemy" && entity.GetHealth() > 0)
        {
            if (attackingList.Contains(collision))
            {
                _attackMode = true;
                animatorController.SetTrigger("Attack");
            }
        }
    }
   

    private void OnTriggerEnter(Collider collision)
    {
        IDamageable entity = collision.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
        
        if (entity != null && collision.tag != "Enemy" && entity.GetHealth() > 0)
        {
            if (!attackingList.Contains(collision)) // Si la lista para almacenar rivales dentro de la hitbox de ataque
            {                                       // no contiene a la entidad, se almacena en ella
                attackingList.Add(collision);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        
        IDamageable entity = collision.GetComponent(typeof(IDamageable)) as IDamageable;
        if (entity != null && collision.tag != "Enemy")
        {
            animatorController.SetBool("AttackMode", false);
            // Si se sale un rival de la hitbox de ataque, se elimina de la lista de enemigos dentro del área de ataque
            attackingList.Remove(collision);
            //_canDamage = false;
            //_attackMode = false;
        }
    }*/

    private void OnDrawGizmosSelected()
    {
        //Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        //
        //Gizmos.matrix = Matrix4x4.TRS(transform.position + attackingBoxPos, transform.rotation, transform.localScale);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(attackingBoxPos, attackingBoxSize);
    }
}
