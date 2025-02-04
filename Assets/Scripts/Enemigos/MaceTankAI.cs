using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))]
public class MaceTankAI : BasicEnemyAI
{
    [Header("Posición de partículas al golpear")]
    public Transform maceTransform;
    private StudioEventEmitter emitterAttack;
    public override GameObject GetFromPool() { return MaceTankPool.Instance.GetMaceTank(); }

    protected override void ReturnEnemyGameObjectToPool() { MaceTankPool.Instance.ReturnMaceTank(this.gameObject); }

    public override void OnAttack()
    {
        base.OnAttack();
        //FMOD
        emitterAttack = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.bigKnightHit, this.gameObject);
        emitterAttack.Play();
        ParticleSystem particleHitGround = MaceHitParticlesPool.Instance.GetMaceHitParticles().GetComponent<ParticleSystem>();
        if (particleHitGround)
        {
            particleHitGround.transform.position = maceTransform.transform.position;
            particleHitGround.Play();
        }
    }
}
