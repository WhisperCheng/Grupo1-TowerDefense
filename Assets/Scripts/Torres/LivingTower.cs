using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingTower : Tower, IDamageable
{
    [Header("Rotaci�n y detecci�n de enemigos")]
    public float rotationSpeed = 5f;
    protected abstract void EnemyDetection();
    public abstract void TakeDamage(float damageAmount);
    public abstract float GetHealth();
    public abstract float GetMaxHealth();
    public abstract void Die();

    protected virtual void LookRotation()
    {
        if (currentTarget != null && rotationPart != null && !locked)
        {
            Vector3 directionToTarget = currentTarget.transform.position - rotationPart.position;
            directionToTarget.y = 0; // Mantenemos solo la rotaci�n en el plano XZ
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Obtenemos solo la rotaci�n en el eje Y
            Vector3 currentEuler = rotationPart.rotation.eulerAngles;
            float targetZRotation = targetRotation.eulerAngles.y; // Usamos el valor en Y para rotaci�n en el plano XZ

            // Se suaviza el �ngulo de rotaci�n en el eje Y
            float smoothYRotation = Mathf.LerpAngle(currentEuler.y, targetZRotation + rotationModelDegrees, Time.deltaTime 
                * rotationSpeed);

            rotationPart.rotation = Quaternion.Euler(currentEuler.x, smoothYRotation, currentEuler.z);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
