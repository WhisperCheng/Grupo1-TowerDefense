using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]
public class AxeTankAI : BasicEnemyAI
{

    private StudioEventEmitter emitter;
    public override GameObject GetFromPool() { return AxeTankPool.Instance.GetAxeTank(); }

    protected override void ReturnEnemyGameObjectToPool() { AxeTankPool.Instance.ReturnAxeTank(this.gameObject); }

    public override void OnAttack()
    {
        base.OnAttack();
        //FMOD
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.bigKnightHit, this.gameObject);
        emitter.Play();
    }
}
