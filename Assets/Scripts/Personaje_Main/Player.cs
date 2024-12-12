using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public float _health;
    private float _currentHealth;
    private float _maxHealth;
    private HealthBar _healthBar;
    public void Die()
    {
        //throw new System.NotImplementedException();
    }

    public float GetHealth()
    {
        return _currentHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        // Daño
        _currentHealth -= damageAmount;
        //OnDamageTaken();

        // Actualizar barra de vida
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _healthBar = GetComponentInChildren<HealthBar>();
        _currentHealth = _health;
        _maxHealth = _health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
