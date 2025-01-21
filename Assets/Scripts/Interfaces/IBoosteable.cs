using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoosteable
{
    public void Boost();
    public bool HasEnoughMoneyForNextBoost();
    public int MaxBoostLevel();
    public int CurrentBoostLevel();
    public int NextBoostMoney();
}
