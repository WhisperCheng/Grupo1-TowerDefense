using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Tower : LivingEntityAI, IDamageable, IPoolable, ILockeable
{
    [Header("Variables Torre")]
    public float range = 10f;
    public float rotationSpeed = 5f;

    [Header("Parte a rotar")]
    public Transform rotationPart;

    //protected List<GameObject> currentTargets = new List<GameObject>();
    protected GameObject currentTarget;

    public float rotationModel = 0f;

    protected bool _hasEnemyAssigned = false;
    protected bool _hasDied = false;
    public bool _locked = true; // Bloqueado / ataque de torres desactivado por defecto. Se activa el ataque cuando se colocan
    protected bool _initialized = false;

    protected int _enemyMask;

    protected abstract void OnDamageTaken(); // Efectos de partículas y efectos visuales al recibir daño
    public abstract void OnAttack(); // Disparar proyectiles, efectos de partículas al golpear, cambiar animación, etc
    public abstract void Die();
    public abstract void TakeDamage(float damageAmount);
    public abstract float GetHealth();
    protected abstract void EnemyDetection();
    public abstract void ReturnToPool();
    public abstract GameObject RestoreToDefault();
    public abstract GameObject GetFromPool();

    void Start()
    {

    }
    public override void Init()
    {
        _locked = true; // Por defecto cuando se crea la torre en el modo Preview con el PlaceManager la torre
                        // estará bloqueada para que no pueda atacar hasta que se coloque.
                        // El PlaceManager se encargará de desbloquear la torre una vez colocada
        _initialized = true;
    }

    protected virtual void LookRotation()
    {
        
        if (currentTarget != null && rotationPart != null)
        {
            Debug.Log(currentTarget + " " + currentTarget.activeSelf + Vector3.Distance(gameObject.transform.position, currentTarget.transform.position));
            Vector3 directionToTarget = currentTarget.transform.position - rotationPart.position;
            directionToTarget.y = 0; // Mantenemos solo la rotación en el plano XZ
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Obtenemos solo la rotación en el eje Z
            Vector3 currentEuler = rotationPart.rotation.eulerAngles;
            float targetZRotation = targetRotation.eulerAngles.y; // Usamos el valor en Y para rotación en el plano XZ

            // Invertimos el ángulo de rotación en el eje Z
            float smoothYRotation = Mathf.LerpAngle(currentEuler.y, targetZRotation + rotationModel, Time.deltaTime * rotationSpeed);

            rotationPart.rotation = Quaternion.Euler(currentEuler.x, smoothYRotation, currentEuler.z);
        }
    }

    public virtual void UnlockTower()
    {
        _locked = false;
    }


    public bool IsLocked()
    {
        return _locked;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
