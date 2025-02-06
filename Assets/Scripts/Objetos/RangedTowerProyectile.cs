using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedTowerProyectile : MonoBehaviour, IProyectile
{
    [Header("Variables proyectil")]
    [SerializeField] protected float damage;
    protected float _extraDamage;
    [SerializeField] protected float proyectileAttackRadius;
    [SerializeField] protected float automaticReturnToPoolTime;

    protected bool inPool = true;

    protected abstract void OnImpactEffects(Collider[] collisions);
    protected abstract void ReturnToPool();

    protected virtual void Start()
    {

    }
    public void AddDamage(float damage) { _extraDamage = damage; }
    private void OnCollisionEnter(Collision collision)
    {
       bool doAttack = collision.gameObject.tag == GameManager.Instance.tagEnemigos 
            || collision.gameObject.tag != GameManager.Instance.tagPlayer;
        if (!inPool && doAttack) // Como es posible que hayan ocasiones en las que el OnCollisionEntese active m�s de una vez muy seguidamente, es necesario saber
        {       // si el proyectil ha sido enviado a la pool para impedir que se vuelva a ejecutar el c�digo si es que se da el caso nuevamente
            OnImpact(collision);
        }
    }

    protected void OnImpact(Collision collision)
    {
        Collider[] enemies = Physics.OverlapSphere(gameObject.transform.position, proyectileAttackRadius, 1 << GameManager.Instance.layerEnemigos);
        
        if (enemies.Length > 0) // Recoger todos los enemigos al rededor dentro del rango de ataque en el impacto y hacerles da�o
        {
            foreach (Collider col in enemies)
            {
                if (col != null && col.gameObject.activeInHierarchy) // Por si acaso el proyectil no est� activo (no deber�a de ocurrir en teor�a)
                {
                    
                    IDamageable damageableEntity = col.gameObject.GetComponent<IDamageable>();
                    DoDamageToCollision(damageableEntity);
                }
            }
        }else // Si por lo que sea el OverlapSphere no consigue recoger/devolver al menos al enemigo con el que se impact�, se aplica el da�o
        {   // aunque sea al enemigo de impacto (collision)
            bool doAttack = collision.gameObject.layer == GameManager.Instance.layerEnemigos;
            if (doAttack)
            {
                IDamageable damageableEntity = collision.gameObject.GetComponent<IDamageable>();
                DoDamageToCollision(damageableEntity);
            }
        }
        OnImpactEffects(enemies);
        CancelInvoke("ReturnToPool"); // Cancelar el invoke previo de enviar a la pool autom�ticamente
        ReturnToPool();
    }

    private void DoDamageToCollision(IDamageable damageableEntity)
    {
        if (damageableEntity != null && damageableEntity.GetHealth() > 0)
        {
            damageableEntity.TakeDamage(damage + _extraDamage);
        }
    }

    public void PopFromPool() // Actualiza el valor para saber si est� en la pool a false y llama al m�todo encargado de meter en la pool al proyectil
    {       // una vez pasado X tiempo
        inPool = false;
        Invoke("ReturnToPool", automaticReturnToPoolTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gameObject.transform.position, proyectileAttackRadius);
    }
#endif
}
