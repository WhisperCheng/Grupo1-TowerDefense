using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToconTower : Tower
{

    private List<GameObject> attackingList = new List<GameObject>();
    protected override void LookRotation() { }

    protected override void EnemyDetection()
    {
        //currentTargets.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, _enemyMask);
        if (colliders.Length > 0)
        {

            foreach (Collider collider in colliders)
            {
                if (!_hasEnemyAssigned) // Si no tiene ningún enemigo asignado, se le asigna uno
                {
                    //currentTargets.Add(collider.gameObject);
                    currentTarget = collider.gameObject;
                    _hasEnemyAssigned = true;

                    if (!attackingList.Contains(currentTarget))
                    {
                        attackingList.Add(currentTarget);
                    }
                    break;
                }
            }
        }
        else // Si no se han detectado enemigos, el target actual es nulo y no le hace focus a nada
        {
            if (attackingList.Contains(currentTarget)) // TODO
            {
                attackingList.Remove(currentTarget);
            }
            //if (currentTarget != null)
            //{
            currentTarget = null;
            _hasEnemyAssigned = false;
            //}
        }

        //if (_hasEnemyAssigned) // Si tiene un enemigo asignado que esé dentro del rango, empieza a atacar
        //{
            //OnAttack();
        //}
    }

    private void DetectarEnemigo() //
    {
        attackingList.Clear();

        Collider[] listaChoques = Physics.OverlapSphere(transform.position, range);

        foreach (Collider enemigo in listaChoques)
        {
            if (enemigo.CompareTag("Enemy"))
            {
                attackingList.Add(enemigo.gameObject);
            }
        }

        if (attackingList.Count > 0 && currentTarget == null)
        {
            currentTarget = attackingList[0];
        }

        if (currentTarget != null && !attackingList.Contains(currentTarget))
        {
            currentTarget = null;
        }
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }

    public override float GetHealth()
    {
        throw new System.NotImplementedException();
    }

    public override void Init()
    {
        throw new System.NotImplementedException();
    }

    public override void OnAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void TakeDamage(float damageAmount)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnDamageTaken()
    {
        throw new System.NotImplementedException();
    }
    protected override void ReturnToPool() {

    }
    public override GameObject RestoreToDefault()
    {
        // TODO
        return gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
