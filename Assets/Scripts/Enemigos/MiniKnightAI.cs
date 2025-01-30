using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(StudioEventEmitter))]

public class MiniKnightAI : BasicEnemyAI
{
    private StudioEventEmitter emitter;


    public override GameObject GetFromPool() { return MiniKnightPool.Instance.GetMiniKnight(); }

    protected override void ReturnEnemyGameObjectToPool() { MiniKnightPool.Instance.ReturnMiniKnight(this.gameObject); }

    public override void OnAttack()
    {
        base.OnAttack();
        //FMOD
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.miniKnightHit, this.gameObject);
        emitter.Play();
    }
}
