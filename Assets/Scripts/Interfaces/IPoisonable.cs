using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoisonable
{
    public void PoisonEntity(float time, float poisonInterval, float poisondamage);
}
