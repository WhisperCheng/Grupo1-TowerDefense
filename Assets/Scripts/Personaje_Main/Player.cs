using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour, IDamageable
{
    public float _health;
    private float _currentHealth;
    private float _maxHealth;
    public HealthBar _healthBar;
    public GameObject playerModel;
    private TimerMuerte timer;
    public CinemachineFreeLook virtualCamera;

    public void Die()
    {
        //throw new System.NotImplementedException();
        timer.DiedPlayerTimer();
        // Desaparecer temporalmente
    }

    public float GetHealth() { return _currentHealth; }
    public float GetMaxHealth() { return _maxHealth; }

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
    public void ReSpawn(Transform destination)
    {
        var delta = destination.position - gameObject.transform.position;

        gameObject.transform.position = destination.transform.position;
        //virtualCamera.PreviousStateIsValid = false;
        //CinemachineCore.Instance.OnTargetObjectWarped(transform, delta);
        //virtualCamera.OnTargetObjectWarped(transform, delta);

        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth = _maxHealth);
    }
    // Start is called before the first frame update
    void Start()
    {
        //_healthBar = GetComponentInChildren<HealthBar>();
        _currentHealth = _health;
        _maxHealth = _health;
        timer = gameObject.GetComponent<TimerMuerte>();
        timer.Player = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
