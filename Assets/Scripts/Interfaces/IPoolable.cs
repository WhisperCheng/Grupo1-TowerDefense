using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    public GameObject RestoreToDefault();

    public GameObject GetFromPool();
    public void ReturnToPool();
}
