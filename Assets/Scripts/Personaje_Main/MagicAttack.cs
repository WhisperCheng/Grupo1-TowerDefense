using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float damage;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Attack(IDamageable damageableEntity)
    {
        damageableEntity.TakeDamage(damage); 
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == GameManager.Instance.tagEnemigos)
        {
            IDamageable damageableEntity = collision.gameObject.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
            if (damageableEntity.GetHealth() > 0) // Si no ha muerto, se sigue atacando
            {
                Attack(damageableEntity);
            }
        }
    }

}
