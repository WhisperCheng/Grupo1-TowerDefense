using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonYewProyectile : RangedTowerProyectile
{
    [Header("Parámetros proyectil venenoso")]
    public float poisonDamage;
    public float poisonInterval;
    public float poisonDuration;
    protected override void ReturnToPool()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; // Reset de las fuezas y velocidad del proyectil
        inPool = true;
        damage = originalDamage;
        PoisonYewProjectilePool.Instance.ReturnPoisonYewProjectile(this.gameObject);
    }

    protected override void OnImpactEffects(Collider[] collisions)
    {
        PoisonEnemies(collisions);

        GameObject poisonParticles = PoisonParticleImpactPool.Instance.GetPoisonYewImpactParticles();
        poisonParticles.transform.position = transform.position;
        poisonParticles.GetComponent<ParticleSystem>().Play();
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
