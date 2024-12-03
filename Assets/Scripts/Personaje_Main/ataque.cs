using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ataque : MonoBehaviour
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
        Debug.Log("Ataque");
        damageableEntity.TakeDamage(damage); 
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.tag == GameManager.Instance.tagEnemigos)
        {
            IDamageable hearthEntity = collision.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
            Attack(hearthEntity);
            Debug.Log("Hola");
        }
    }
}
