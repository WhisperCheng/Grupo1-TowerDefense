using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ataque : MonoBehaviour
{
    // Start is called before the first frame update
    private float _currentCooldown = 0f;
    public float cooldown = 1f;
    private bool canAttack = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ManageCooldown();
    }

    private void Attack(IDamageable damageableEntity)
    {
        Debug.Log("Ataque");
        damageableEntity.TakeDamage(1f); 
    }

    protected void ManageCooldown()
    {
        _currentCooldown -= Time.deltaTime;
        if (!canAttack && _currentCooldown <= 0)
        {
            canAttack = true;
            _currentCooldown = 0;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.tag == GameManager.Instance.tagEnemigos)
        {
            IDamageable hearthEntity = collision.GetComponent(typeof(IDamageable)) as IDamageable; // versión no genérica
            Attack(hearthEntity);
            _currentCooldown = cooldown; // Reset del cooldown
            canAttack = false;
            Debug.Log("Hola");
        }
    }
}
