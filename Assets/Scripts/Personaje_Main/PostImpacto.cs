using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PostImpacto : MonoBehaviour
{
    public UnityEvent OnDefinePoolType;
    private void OnParticleSystemStopped()
    {
        OnDefinePoolType?.Invoke();
    }

    public void SetToMagicImpactPool()
    {
        MagicImpactPool.Instance.ReturnMagicImpact(this.gameObject);
    }

    public void SetToEvilMagicImpactPool()
    {
        EvilMagicImpactPool.Instance.ReturnEvilMagicImpact(this.gameObject);
    }
}
