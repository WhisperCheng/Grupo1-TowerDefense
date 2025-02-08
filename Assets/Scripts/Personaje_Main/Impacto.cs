using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(StudioEventEmitter))] 

public class Impacto : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private GameObject particulasImpacto;
    [SerializeField] private GameObject trail;
    [SerializeField] private GameObject trail2;
    private TrailRenderer TrailRenderer;
    private TrailRenderer TrailRenderer2;

    public UnityEvent OnDefineImpactType;

    //FMOD
    private StudioEventEmitter emitter;

    private void Start()
    {
        TrailRenderer = trail.GetComponent<TrailRenderer>();
        TrailRenderer2 = trail2.GetComponent<TrailRenderer>();


        //FMOD emitter parameters
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.magicImpact, this.gameObject);
        emitter.OverrideAttenuation = true;
        emitter.OverrideMinDistance = 1;
        emitter.OverrideMaxDistance = 100;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Collide();
    }

    private void OnTriggerEnter(Collider other)
    {
        Collide();
    }

    private void Collide()
    {
        OnDefineImpactType?.Invoke();

        // Limpiar los Trail Renderers
        if (TrailRenderer != null)
        {
            TrailRenderer.enabled = false;
            TrailRenderer.Clear();
            TrailRenderer.enabled = true;
        }

        if (TrailRenderer2 != null)
        {
            TrailRenderer2.enabled = false;
            TrailRenderer2.Clear();
            TrailRenderer2.enabled = true;
        }

        //FMOD
        emitter.Play();
    }

    public void SetMagicImpact()
    {
        GameObject impacto = MagicImpactPool.Instance.GetMagicImpact();
        impacto.transform.position = transform.position;
        // Retornar el proyectil a la pool
        MagicProjectilePool.Instance.ReturnMagicProjectile(this.gameObject);
    }

    public void SetEvilMagicImpact()
    {
        GameObject impacto = EvilMagicImpactPool.Instance.GetEvilMagicImpact();
        impacto.transform.position = transform.position;

        // Retornar el proyectil a la pool
        EvilMagicProjectilePool.Instance.ReturnMagicProjectile(this.gameObject);
    }

}
