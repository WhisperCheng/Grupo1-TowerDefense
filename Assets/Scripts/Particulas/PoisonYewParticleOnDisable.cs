using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonYewParticleOnDisable : MonoBehaviour
{
    private void OnDisable()
    {
        PoisonParticleImpactPool.Instance.ReturnPoisonYewImpactParticles(gameObject);
    }
}
