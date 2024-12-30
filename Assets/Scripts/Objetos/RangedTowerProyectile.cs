using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedTowerProyectile : MonoBehaviour
{
    [Header("Variables proyectil")]
    [SerializeField] private float damage;
    [SerializeField] private float proyectileAttackRadius;
    [SerializeField] private float automaticReturnToPoolTime;

    protected bool inPool = true;

    protected abstract void OnImpactEffects(Collider[] collisions);
    protected abstract void ReturnToPool();
    private void OnCollisionEnter(Collision collision)
    {
        bool doAttack = collision.gameObject.tag == GameManager.Instance.tagEnemigos || collision.gameObject.tag != GameManager.Instance.tagPlayer;
        if (!inPool && doAttack) // Como es posible que hayan ocasiones en las que el OnCollisionEntese active más de una vez muy seguidamente, es necesario saber
        {       // si el proyectil ha sido enviado a la pool para impedir que se vuelva a ejecutar el código si es que se da el caso nuevamente
            OnImpact();
        }
    }

    protected void OnImpact()
    {
        Collider[] enemies = Physics.OverlapSphere(gameObject.transform.position, proyectileAttackRadius, 1 << GameManager.Instance.layerEnemigos);
        if (enemies.Length > 0)
        {
            foreach (Collider col in enemies)
            {
                if (col != null && col.gameObject.activeInHierarchy) // Por si acaso el proyectil no está activo (no debería de ocurrir en teoría)
                {
                    IDamageable damageableEntity = col.gameObject.GetComponent<IDamageable>();
                    if (damageableEntity != null && damageableEntity.GetHealth() > 0)
                    {
                        damageableEntity.TakeDamage(damage);
                    }
                }
            }
        }
        OnImpactEffects(enemies);
        CancelInvoke("ReturnToPool"); // Cancelar el invoke previo de enviar a la pool automáticamente
        ReturnToPool();
    }

    public void PopFromPool() // Actualiza el valor para saber si está en la pool a false y llama al método encargado de meter en la pool al proyectil
    {       // una vez pasado X tiempo
        inPool = false;
        Invoke("ReturnToPool", automaticReturnToPoolTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gameObject.transform.position, proyectileAttackRadius);
    }
}
