using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostImpacto : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        MagicImpactPool.Instance.ReturnMagicImpact(this.gameObject);
    }
}
