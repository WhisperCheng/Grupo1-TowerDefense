using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StudioEventEmitter))]

public class RoseTower : RangedTower
{
    //FMOD
    private StudioEventEmitter emitter;

    public override void ShootProyectileEvent()
    {
        base.ShootProyectileEvent();
        if (currentTarget != null)
        {
            GameObject proyectile = ThornRoseProjectilePool.Instance.GetThornRoseProjectile();
            IProyectile proyectil = proyectile.GetComponent<IProyectile>();

            //FMOD
            emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.roseShoot, this.gameObject);
            emitter.Play();

            bool hasSomeBoostApplied = _boostIndex != -1;
            if (boostPrices.Count > 0 && proyectil != null && hasSomeBoostApplied) // Añadir daño extra a los proyectiles según las mejoras
            {
                proyectile.GetComponent<IProyectile>().AddDamage(boostPrices[_boostIndex].damageAddition);
            }

            // Para apuntar hacia el centro del enemigo
            NavMeshAgent agent = currentTarget.GetComponent<NavMeshAgent>();
            float offsetYTargetPosition = (agent != null ? agent.height / 2 : 0);
            Vector3 offsetY = Vector3.up * offsetYTargetPosition;

            // Trayectora sin predicción de movimiento
            //Vector3 targetPosition = currentTarget.transform.position + offsetY;

            proyectile.transform.position = shooterSource.position;
            proyectile.transform.rotation = shooterSource.rotation;
            Vector3 predictivePosition =                           // Trayectoria predictiva
                ProyectileUtils.ShootingInterception
                .CalculateInterceptionPoint(shootingSpeed, shooterSource.transform, currentTarget.transform, offsetY);
            ProyectileUtils.ThrowProyectileAtLocation(shooterSource.transform, proyectile, predictivePosition, shootingSpeed);

            //Partículas de escupir
            ParticleSystem particleSpit = RoseSpitParticlePool.Instance.GetRoseSpitParticles().GetComponent<ParticleSystem>();
            particleSpit.transform.position = shooterSource.position + shooterSource.forward * 1.1f;
            particleSpit.transform.rotation = shooterSource.rotation;
            particleSpit.transform.Rotate(Vector3.up * 180 + Vector3.right * -90);
            particleSpit.Play();
        }
    }

    public override GameObject GetFromPool()
    {
        return ThornRosePool.Instance.GetThornRose();
    }

    public override void Die()
    {
        ReturnToPool();
    }

    public override void ReturnToPool()
    {
        base.ReturnToPool();
        ThornRosePool.Instance.ReturnThornRose(this.gameObject);
    }

}
