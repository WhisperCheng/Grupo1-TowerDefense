using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YewTower : RangedTower
{
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{

    //}

    public override void ShootProyectileEvent()
    {
        

        if (currentTarget != null)
        {
            GameObject proyectile = PoisonYewProjectilePool.Instance.GetPoisonYewProjectile();

            // Para apuntar hacia el centro del enemigo
            float offsetYTargetPosition = currentTarget.GetComponent<NavMeshAgent>() != null ?
                currentTarget.GetComponent<NavMeshAgent>().height / 2 : 0;

            Vector3 targetPosition = currentTarget.transform.position + new Vector3(0, offsetYTargetPosition, 0);

            proyectile.transform.position = shooterSource.position;
            ProyectileUtils.ThrowBallAtTargetLocation(shooterSource.transform, proyectile, targetPosition, 20);
        }
    }
}