using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingTower : Tower, IDamageable, IBoosteable
{
    [Header("Rotación y detección de enemigos")]
    public float rotationSpeed = 5f;

    [Header("Lista de mejoras y precios")]
    [SerializeField] protected List<BoostInfo> boostPrices;
    protected int _boostIndex = -1;
    protected abstract void EnemyDetection();
    public abstract void TakeDamage(float damageAmount);
    public abstract float GetHealth();
    public abstract float GetMaxHealth();
    public abstract void Die();

    public override void Init()
    {
        base.Init();
        _boostIndex = -1;
    }
    public virtual void Boost()
    {
        if (NextBoostMoney() != -1) // Mientras hayan boosts disponibles
        {
            _boostIndex++;
            cooldown = _originalCooldown - boostPrices[_boostIndex].cooldownReducer;
            if (cooldown < 0)
                cooldown = 0;
            Debug.Log("Boost");
        }
    }
    public bool HaEnoughMoneyForNextBoost()
    { // Si el dinero que hay es mayor o igual al precio de la siguiente mejora
        return MoneyManager.Instance.GetMoney() >= NextBoostMoney() && NextBoostMoney() != -1;
    }
    public int MaxBoostLevel() { return boostPrices.Count - 1; }

    public int CurrentBoostLevel() { return _boostIndex; }

    public int NextBoostMoney()
    {
        if (_boostIndex < MaxBoostLevel()) { return boostPrices[_boostIndex+1].price; } // Si existe un siguiente boost, se retorna el precio
        return -1; // Si no hay siguientes boost, se retorna -1
    }

    protected virtual void LookRotation()
    {
        if (currentTarget != null && rotationPart != null && !locked)
        {
            Vector3 directionToTarget = currentTarget.transform.position - rotationPart.position;
            directionToTarget.y = 0; // Mantenemos solo la rotación en el plano XZ
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Obtenemos solo la rotación en el eje Y
            Vector3 currentEuler = rotationPart.rotation.eulerAngles;
            float targetZRotation = targetRotation.eulerAngles.y; // Usamos el valor en Y para rotación en el plano XZ

            // Se suaviza el ángulo de rotación en el eje Y
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
