using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestHearthIA : LivingEntityAI, IDamageable
{
    [Header("Vida")] // Vida
    public float health;
    public HealthBar _healthBar;

    private float _currentHealth;
    private float _maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Init()
    {
        _currentHealth = health;
        _maxHealth = health;
        //_healthBar = GetComponentInChildren<HealthBar>();
        GameManager.Instance.addForestHearth();
    }

    public void TakeDamage(float damageAmount)
    {
        // Dañar corazón
        _currentHealth -= damageAmount;

        // TODO ? Spawn particulas de golpe?

        // Actualizar barra de vida
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
        GameManager.Instance.ForestHearthAmount--;
        // TODO: Efecto de partículas, llamar al GameManager para actualizar la info del corazón del bosque
    }

    public float GetHealth() { return _currentHealth; }
    public float GetMaxHealth() { return _maxHealth; }
}
