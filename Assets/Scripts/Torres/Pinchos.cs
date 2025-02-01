using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinchos : StaticTower
{
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
            life -= damageOnUsed;
            if (life <= 0)
            {
                ReturnToPool();
            } else
            {
                _healthBar.UpdateHealthBar(_maxLife, life);
            }
            
        }
    }

    public override void ReturnToPool() {
        if (_initialized && _loaded)
        {
            life = _maxLife;
            _healthBar.ResetHealthBar(); // Actualizamos la barra de salud
            SpikeTrapPool.Instance.ReturnSpikeTrap(gameObject);
        }
    }

    public override GameObject RestoreToDefault() {
        if (!locked)
        {
            Init();
        }
        return gameObject;
    }

    public override GameObject GetFromPool() { return SpikeTrapPool.Instance.GetSpikeTrap(); }
}
