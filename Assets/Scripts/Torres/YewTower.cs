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

            Vector3 targetPosition = currentTarget.transform.position + new Vector3(0, offsetYTargetPosition, 0);

            proyectile.transform.position = shooterSource.position;
            ProyectileUtils.ThrowProyectileAtTargetLocation(shooterSource.transform, proyectile, targetPosition, shootingSpeed);
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
