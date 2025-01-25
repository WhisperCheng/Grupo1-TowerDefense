using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(StudioEventEmitter))]

public class PoisonYewProyectile : RangedTowerProyectile
{
    //FMOD
    private StudioEventEmitter emitter;

    [Header("Parámetros proyectil venenoso")]
    public float poisonDamage;
    public float poisonInterval;
    public float poisonDuration;

    //He añadido esta función porque si no el sonido sonaba muy tarde, una vez la explosión ya se había hecho,
    // revisar si esto trae problemas de implementación - sergio
    protected override void Start()
    {
        base.Start();
        //Esta linea lo que hace es definir el sonido que va a sonar, básicamente, por lo general puede ir con el
        //mismo emitter.play, pero si se puede definir lo antes posible mejor
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.bombardierExplosion, this.gameObject);
    }
    protected override void ReturnToPool()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; // Reset de las fuezas y velocidad del proyectil
        inPool = true;
        _extraDamage = 0;
        PoisonYewProjectilePool.Instance.ReturnPoisonYewProjectile(this.gameObject);
    }

    protected override void OnImpactEffects(Collider[] collisions)
    {
        PoisonEnemies(collisions);

        GameObject poisonParticles = PoisonParticleImpactPool.Instance.GetPoisonYewImpactParticles();
        poisonParticles.transform.position = transform.position;
        poisonParticles.GetComponent<ParticleSystem>().Play();
        //FMOD
        emitter.Play();

    }

    private void PoisonEnemies(Collider[] collisions)
    {
        if(collisions.Length > 0)
            foreach (Collider col in collisions){
                if (col.gameObject.activeSelf)
                {
                    IPoisonable poisonableEntity = col.GetComponent<IPoisonable>();
                    poisonableEntity.PoisonEntity(poisonDuration, poisonInterval, poisonDamage);
                }
        }
    }
}
