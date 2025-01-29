using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostShootingParticles : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        ShootingMagicParticlesPool.Instance.ReturnMagicShootingParticle(this.gameObject);
    }
}
