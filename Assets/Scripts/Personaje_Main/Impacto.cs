using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impacto : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] GameObject particulasImpacto;
    [SerializeField] GameObject trail;
    [SerializeField] GameObject trail2;
    TrailRenderer TrailRenderer;
    TrailRenderer TrailRenderer2;
    private void Start()
    {
        TrailRenderer = trail.GetComponent<TrailRenderer>();
        TrailRenderer2 = trail2.GetComponent<TrailRenderer>();
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

    }

}
