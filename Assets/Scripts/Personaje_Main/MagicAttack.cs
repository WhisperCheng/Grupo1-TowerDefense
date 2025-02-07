using System.Collections.Generic;
using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float damage;
    [SerializeField] private List<int> layersToDamage;
    private int damageLayers;
    void Start()
    {
        foreach(int layer in layersToDamage)
        {
            damageLayers |= 1 << layer;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void Attack(IDamageable damageableEntity)
    {
        damageableEntity.TakeDamage(damage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Se comprueba si el layer de la colisión está contenida en la máscara de layers a dañar
        bool checkLayer = (damageLayers & (1 << collision.gameObject.layer)) != 0;
        //if (collision.gameObject.tag == GameManager.Instance.tagEnemigos)
        if (checkLayer)
        {
            IDamageable damageableEntity = collision.gameObject.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
            if (damageableEntity != null && damageableEntity.GetHealth() > 0) // Si no ha muerto, se sigue atacando
            {
                Attack(damageableEntity);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Se comprueba si el layer de la colisión está contenida en la máscara de layers a dañar
        bool checkLayer = (damageLayers & (1 << other.gameObject.layer)) != 0;
        if (checkLayer)
        {
            IDamageable damageableEntity = other.gameObject.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
            if (damageableEntity != null && damageableEntity.GetHealth() > 0) // Si no ha muerto, se sigue atacando
            {
                Attack(damageableEntity);
            }
        }
    }
}
