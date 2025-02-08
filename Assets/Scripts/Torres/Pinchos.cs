using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinchos : StaticTower, IDamageable
{
    [Header("Parámetros Pinchos")]
    [SerializeField] private float life;
    [SerializeField] private float damage;
    [SerializeField] private float damageOnUsed;
    private float _maxLife;
    private HealthBar _healthBar;
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
        base.Init();
        _maxLife = life;
        _healthBar = GetComponentInChildren<HealthBar>();
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.tag == GameManager.Instance.tagEnemigos && !collision.isTrigger)
        {
            IDamageable damageableEntity = collision.GetComponent(typeof(IDamageable)) as IDamageable;
            damageableEntity.TakeDamage(damage);
            TakeDamage(damageOnUsed);
        }
    }

    public override void ReturnToPool()
    {
        if (_initialized && _loaded)
        {
            life = _maxLife;
            _healthBar.ResetHealthBar(); // Actualizamos la barra de salud
            SpikeTrapPool.Instance.ReturnSpikeTrap(gameObject);
        }
    }

    public override GameObject RestoreToDefault()
    {
        if (!locked)
        {
            Init();
        }
        return gameObject;
    }

    public override GameObject GetFromPool() { return SpikeTrapPool.Instance.GetSpikeTrap(); }

    public void Die() { ReturnToPool(); }

    public void TakeDamage(float damageAmount)
    {
        life -= damageAmount; // Quitar vida a la trampa
        if (life <= 0)
        {
            Die();
        }
        else
        {
            _healthBar.UpdateHealthBar(_maxLife, life);
        }
    }

    public float GetHealth() { return life; }

    public float GetMaxHealth() { return _maxLife; }
}
