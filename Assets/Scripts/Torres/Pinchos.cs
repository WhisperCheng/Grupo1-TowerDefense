using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinchos : StaticTower
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

    public override void ReturnToPool() { SpikeTrapPool.Instance.ReturnSpikeTrap(gameObject); }

    public override GameObject RestoreToDefault() { return gameObject; }

    public override GameObject GetFromPool() { return SpikeTrapPool.Instance.GetSpikeTrap(); }
}
