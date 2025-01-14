using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectorTorre : StaticTower
{
    [Header("Dinero a recolectar")]
    [SerializeField] protected int recolectMoneyAmount;
    protected Animator _animator;
    public override GameObject GetFromPool() { return ResourceTowerPool.Instance.GetResourceTower(); }

    public override GameObject RestoreToDefault() { return gameObject; }

    public override void ReturnToPool() { ResourceTowerPool.Instance.ReturnResourceTower(gameObject); }
    protected override void UpdateCurrentCooldown()
    {
        _animator.SetFloat("Cooldown", _currentCooldown);
        base.UpdateCurrentCooldown();
    }

    public override void Init()
    {
        base.Init();
        _animator = GetComponent<Animator>();
        _animator.SetFloat("Cooldown", _currentCooldown);
    }

    public void AddGemsEvent()
    {
        MoneyManager.Instance.AddMoney(recolectMoneyAmount);
        _currentCooldown = cooldown;
        Debug.Log(MoneyManager.Instance.GetMoney());
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentCooldown();
    }
}
