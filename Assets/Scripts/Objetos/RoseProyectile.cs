using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoseProyectile : RangedTowerProyectile
{
    protected override void ReturnToPool()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; // Reset de las fuezas y velocidad del proyectil
        inPool = true;
        _extraDamage = 0;
        ThornRoseProjectilePool.Instance.ReturnThornRoseProjectile(this.gameObject);
    }

    protected override void OnImpactEffects(Collider[] collisions)
    {

    }
}
