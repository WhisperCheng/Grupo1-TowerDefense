using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniKnightAI : BasicEnemyAI
{
    public override GameObject GetFromPool() { return MiniKnightPool.Instance.GetMiniKnight(); }

    protected override void ReturnEnemyGameObjectToPool() { MiniKnightPool.Instance.ReturnMiniKnight(this.gameObject); }

    public override void OnAttack()
    {
        base.OnAttack();
        // SONIDO : Ataque
    }
}
