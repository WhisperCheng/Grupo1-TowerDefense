using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Tower : LivingEntityAI, IDamageable
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

    protected int _enemyMask;

    protected abstract void OnDamageTaken(); // Efectos de part�culas y efectos visuales al recibir da�o
    public abstract void OnAttack(); // Disparar proyectiles, efectos de part�culas al golpear, cambiar animaci�n, etc
    public abstract void Die();
    public abstract void TakeDamage(float damageAmount);
    public abstract float GetHealth();
    protected abstract void EnemyDetection();

    protected void LookRotation()
    {
        if (currentTarget != null && rotationPart != null)
        {
            Vector3 directionToTarget = currentTarget.transform.position - rotationPart.position;
            directionToTarget.y = 0; // Mantenemos solo la rotaci�n en el plano XZ
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Obtenemos solo la rotaci�n en el eje Z
            Vector3 currentEuler = rotationPart.rotation.eulerAngles;
            float targetZRotation = targetRotation.eulerAngles.y; // Usamos el valor en Y para rotaci�n en el plano XZ

            // Invertimos el �ngulo de rotaci�n en el eje Z
            float smoothYRotation = Mathf.LerpAngle(currentEuler.y, targetZRotation + rotationModel, Time.deltaTime * rotationSpeed);

            rotationPart.rotation = Quaternion.Euler(currentEuler.x, smoothYRotation, currentEuler.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
