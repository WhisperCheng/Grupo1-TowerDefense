using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingTower : Tower, IDamageable, IBoosteable
{
    [Header("Coronas de mejoras")]
    [SerializeField] protected CrownsData _crownData;
    [SerializeField] protected GameObject crownHolder;
    [Range(0, 1)]
    [SerializeField] protected float crownElevation;
    [SerializeField] protected float animationTime;
    [SerializeField] protected LeanTweenType animationType;

    [Header("Rotación y detección de enemigos")]
    public float rotationSpeed = 5f;

    [Header("Lista de mejoras y precios")]
    [SerializeField] protected List<BoostInfo> boostPrices;
    protected int _boostIndex = -1;
    protected int _originalMoney = 0;
    protected abstract void EnemyDetection();
    public abstract void TakeDamage(float damageAmount);
    public abstract float GetHealth();
    public abstract float GetMaxHealth();
    public abstract void Die();

    protected virtual void Start()
    {
        LeanTween.moveLocalY(crownHolder, crownHolder.transform.localPosition.y + crownElevation, animationTime)
            .setLoopPingPong().setEase(animationType);
    }

    public override void Init()
    {
        base.Init();
        _boostIndex = -1;
        _originalMoney = money;
    }
    public virtual void Boost()
    {
        if (NextBoostMoney() != -1) // Mientras hayan boosts disponibles
        {
            _boostIndex++;
            cooldown = _originalCooldown - boostPrices[_boostIndex].cooldownReducer;

            money += _originalMoney; // Se suma al precio lo que costaba al principio y luego si se
                                                         // vende la torre se devuelve esta suma dividida a la mitad, ya que se 
            if (cooldown < 0)                               // ha añadido el dinero de la mejora
                cooldown = 0;

            const int firstLevel = 0;
            const int lastLevel = 1;
            switch (_boostIndex) // Crea una corona dependiendo del nivel de mejora que tiene la torre
            {
                case firstLevel:
                    if (crownHolder != null)
                        CreateCrown(_crownData.CrownLevelOne);
                    break;
                case lastLevel:
                    if (crownHolder != null)
                    {
                        RemoveExistentCrowns();
                        CreateCrown(_crownData.CrownLevelTwo);
                    }
                    break;

                default:
                    break;
            }
        }
    }

    protected void RemoveExistentCrowns()
    {
        for (int i = 0; i < crownHolder.transform.childCount; i++)
        {
            Transform child = crownHolder.transform.GetChild(i);
            if (child.gameObject.tag == GameManager.Instance.tagCoronas) // Eliminar la corona que tenga la planta
            {
                Destroy(child.gameObject);
            }
        }
    }
    protected void CreateCrown(GameObject crown)
    {
        GameObject newCrown = Instantiate(crown, crownHolder.transform.position, crownHolder.transform.rotation);
        newCrown.transform.parent = crownHolder.transform;
    }
    public bool HasEnoughMoneyForNextBoost()
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
}
