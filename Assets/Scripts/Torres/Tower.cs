using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Tower : LivingEntityAI, IPoolable
{
    [Header("Variables Torre")]
    public float range = 10f;
    public float rotationSpeed = 5f;
    //public float towerRadiusSize = 1f;

    [Header("Parte a rotar")]
    public Transform rotationPart;

    [Header("Ataque")] //Ataque
    public float cooldown = 1f;
    protected float _currentCooldown;

    //protected List<GameObject> currentTargets = new List<GameObject>();
    protected GameObject currentTarget;

    public float rotationModel = 0f;

    protected bool _hasEnemyAssigned = false;
    public bool _locked = true; // Bloqueado / ataque de torres desactivado por defecto. Se activa el ataque cuando se coloca la torre.

    protected bool _initialized = false; // Initialized se usa para indicar si la torre ha sido cargada dentro de la escena (en memoria),
                                         // ya sea estando desactivada o activada.

    protected bool _loaded = false; // Loaded se llama cuando la torre está empezando a ser colocada (el objeto ya se ha cargado y es visible, aunque sea
                                    // en modo previsualización). Se usa para saber si la torre ya ha sido colocada en el mundo y de esa forma saber si
                                    // al devolverla a la pool se puede devolver con sus valores reiniciados en caso de que esos valores no sean nulos.
    protected int _enemyMask;

    protected virtual void OnDamageTaken() { } // Efectos de partículas y efectos visuales al recibir daño
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
        _enemyMask = 1 << GameManager.Instance.layerEnemigos;
        _currentCooldown = cooldown; // Reset del cooldown, simulando que desde que se coloque la torre se va preparando hasta poder atacar
        _initialized = true;
    }

    protected virtual void LookRotation()
    {
        if (currentTarget != null && rotationPart != null && !_locked)
        {
            Vector3 directionToTarget = currentTarget.transform.position - rotationPart.position;
            directionToTarget.y = 0; // Mantenemos solo la rotación en el plano XZ
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            
            // Obtenemos solo la rotación en el eje Y
            Vector3 currentEuler = rotationPart.rotation.eulerAngles;
            float targetZRotation = targetRotation.eulerAngles.y; // Usamos el valor en Y para rotación en el plano XZ
            
            // Se suaviza el ángulo de rotación en el eje Y
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

    public void SetLoaded(bool loaded)
    {
        _loaded = loaded;
    }

    protected virtual void UpdateCurrentCooldown()
    {
        if (_currentCooldown > 0)
        {
            _currentCooldown -= Time.deltaTime;
        }
        if (_currentCooldown < 0)
        {
            _currentCooldown = 0;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
        if (currentTarget != null)
        Gizmos.DrawLine(currentTarget.transform.position, currentTarget.transform.position + Vector3.up*7);
        Gizmos.color = Color.gray;
        //Gizmos.DrawWireSphere(transform.position, towerRadiusSize);
    }
}
