using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathParticleOnDisable : MonoBehaviour
{
    private void OnDisable()
    {
        EnemyDeathParticlesPool.Instance.ReturnEnemyDeathParticles(gameObject);
    }
}
