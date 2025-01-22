using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StudioEventEmitter))] 

public class Impacto : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private GameObject particulasImpacto;
    [SerializeField] private GameObject trail;
    [SerializeField] private GameObject trail2;
    private TrailRenderer TrailRenderer;
    private TrailRenderer TrailRenderer2;

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
    void OnCollisionEnter(Collision collision)
    {
        GameObject impacto = MagicImpactPool.Instance.GetMagicImpact();
        impacto.transform.position = transform.position;
        
        // Retornar el proyectil a la pool
        MagicProjectilePool.Instance.ReturnMagicProjectile(this.gameObject);

        

        // Limpiar los Trail Renderers
        if (TrailRenderer != null)
        {
            TrailRenderer.Clear();
            TrailRenderer.enabled = false;
            TrailRenderer.enabled = true;
        }

        if (TrailRenderer2 != null)
        {
            TrailRenderer2.Clear();
            TrailRenderer2.enabled = false;
            TrailRenderer2.enabled = true;
        }

        //FMOD
        emitter.Play();
    }

}
