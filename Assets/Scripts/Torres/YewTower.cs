using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YewTower : RangedTower
{
    public override void ShootProyectileEvent()
    {
        base.ShootProyectileEvent();
        if (currentTarget != null)
        {
            GameObject proyectile = PoisonYewProjectilePool.Instance.GetPoisonYewProjectile();
            IProyectile proyectil = proyectile.GetComponent<IProyectile>();
            bool hasSomeBoostApplied = _boostIndex != -1;
            if (boostPrices.Count > 0 && proyectil != null && hasSomeBoostApplied) // Añadir daño extra a los proyectiles según las mejoras
            {
                proyectil.AddDamage(boostPrices[_boostIndex].damageAddition);
            }

            // Para apuntar hacia el centro del enemigo
            float offsetYTargetPosition = currentTarget.GetComponent<NavMeshAgent>() != null ?
                currentTarget.GetComponent<NavMeshAgent>().height / 2 : 0;
            Vector3 offsetY = Vector3.up * offsetYTargetPosition;

            // Trayectora sin predicción de movimiento
            //Vector3 targetPosition = currentTarget.transform.position + offsetY;

            proyectile.transform.position = shooterSource.position; 
            proyectile.transform.rotation = Quaternion.Euler(shooterSource.forward); 
            Vector3 predictivePosition =                           // Trayectoria predictiva
                ProyectileUtils.ShootingInterception
                .CalculateInterceptionPoint(shootingSpeed, shooterSource.transform, currentTarget.transform, offsetY);
            ProyectileUtils.ThrowProyectileAtLocation(shooterSource.transform, proyectile, predictivePosition, shootingSpeed);
        }
    }
    public override GameObject GetFromPool()
    {
        return PoisonYewPool.Instance.GetPoisonYew();
    }
    public override void Die()
    {
        ReturnToPool();
    }
    public override void ReturnToPool()
    {
        base.ReturnToPool();
        PoisonYewPool.Instance.ReturnPoisonYew(this.gameObject);
    }
}
