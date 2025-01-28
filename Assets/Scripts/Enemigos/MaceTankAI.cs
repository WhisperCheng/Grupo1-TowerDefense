using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaceTankAI : BasicEnemyAI
{
    public override GameObject GetFromPool() { return MaceTankPool.Instance.GetMaceTank(); }

    protected override void ReturnEnemyGameObjectToPool() { MaceTankPool.Instance.ReturnMaceTank(this.gameObject); }

    public override void OnAttack()
    {
        base.OnAttack();
        // SONIDO : Ataque
    }
}
