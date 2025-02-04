using UnityEngine;
using UnityEngine.Events;

public class OnDisableParticles : MonoBehaviour
{
    public UnityEvent onDisableParticle;

    public void ReturnMaceHitParticle()
    {
        MaceHitParticlesPool.Instance.ReturntMaceHitParticles(gameObject);
    }

    public void ReturnRoseSpitParticles()
    {
        RoseSpitParticlePool.Instance.ReturnRoseSpitParticles(gameObject);
    }

    public void ReturnHitParticles()
    {
        HitParticlePool.Instance.ReturnParticles(gameObject);
    }

    public void DestroyDeathParticles()
    {
        Destroy(this.gameObject);
    }

    private void OnDisable()
    {
        onDisableParticle.Invoke();
    }
}
