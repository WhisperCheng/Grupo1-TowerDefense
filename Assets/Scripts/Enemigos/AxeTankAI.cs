using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeTankAI : BasicEnemyAI
{
    public override GameObject GetFromPool() { return AxeTankPool.Instance.GetAxeTank(); }

    protected override void ReturnEnemyGameObjectToPool() { AxeTankPool.Instance.ReturnAxeTank(this.gameObject); }

    public override void OnAttack()
    {
        base.OnAttack();
        // SONIDO : Ataque
    }
}
