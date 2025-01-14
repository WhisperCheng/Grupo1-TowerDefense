using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Tower : LivingEntityAI, IPoolable
{
    [Header("Precio")]
    [SerializeField] protected int money;

    [Header("Rango y Radio de tamaño")]
    [SerializeField] protected float range = 1f;
    [SerializeField] protected float towerRadiusSize = 1f;

    [Header("Parte a rotar")]
    [SerializeField] protected Transform rotationPart;
    [SerializeField] protected float rotationModelDegrees = 0f;

    [Header("Cooldown")] //Ataque
    [SerializeField] protected float cooldown = 1f;
    protected float _currentCooldown;

    //protected List<GameObject> currentTargets = new List<GameObject>();
    protected GameObject currentTarget;

    public int Money { get { return money; } private set { } }

    protected bool _hasEnemyAssigned = false;
    public bool locked = true; // Bloqueado / ataque de torres desactivado por defecto. Se activa el ataque cuando se coloca la torre.

    protected bool _initialized = false; // Initialized se usa para indicar si la torre ha sido cargada dentro de la escena (en memoria),
                                         // ya sea estando desactivada o activada.
    public bool placed = false;

    protected bool _loaded = false; // Loaded se llama cuando la torre está empezando a ser colocada (el objeto ya se ha cargado y es visible, aunque sea
                                    // en modo previsualización). Se usa para saber si la torre ya ha sido colocada en el mundo y de esa forma saber si
                                    // al devolverla a la pool se puede devolver con sus valores reiniciados en caso de que esos valores no sean nulos.
    protected int _enemyMask;

    protected virtual void OnDamageTaken() { } // Efectos de partículas y efectos visuales al recibir daño

    //public abstract void OnAttack(); // Disparar proyectiles, efectos de partículas al golpear, cambiar animación, etc
    public abstract void ReturnToPool();
    public abstract GameObject RestoreToDefault();
    public abstract GameObject GetFromPool();

    void Start()
    {

    }

    public override void Init()
    {
        locked = true; // Por defecto cuando se crea la torre en el modo Preview con el PlaceManager la torre
                       // estará bloqueada para que no pueda atacar hasta que se coloque.
                       // El PlaceManager se encargará de desbloquear la torre una vez colocada
        _enemyMask = 1 << GameManager.Instance.layerEnemigos;
        _currentCooldown = cooldown; // Reset del cooldown, simulando que desde que se coloque la torre se va preparando hasta poder atacar
        _initialized = true;
    }

    public virtual void UnlockTower()
    {
        locked = false;
        MoneyManager.Instance.RemoveMoney(money);
    }

    public bool IsLocked() { return locked; }
    public float GetRange() { return range; }

    public void SetLoaded(bool loaded) { _loaded = loaded; }

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
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, range);
        if (currentTarget != null)
            Gizmos.DrawLine(currentTarget.transform.position, currentTarget.transform.position + Vector3.up * 7);
        //Gizmos.color = Color.gray;
        //Gizmos.DrawWireSphere(transform.position, towerRadiusSize);
    }
}
