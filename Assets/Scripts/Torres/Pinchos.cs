using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinchos : MonoBehaviour
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

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.tag == GameManager.Instance.tagEnemigos && !collision.isTrigger)
        {
            IDamageable damageableEntity = collision.GetComponent(typeof(IDamageable)) as IDamageable;
            damageableEntity.TakeDamage(damage);
        }
    }
}
