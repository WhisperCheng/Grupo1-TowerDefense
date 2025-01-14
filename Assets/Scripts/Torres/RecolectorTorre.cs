using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectorTorre : StaticTower
{
    public override GameObject GetFromPool() { return ResourceTowerPool.Instance.GetResourceTower(); }

    public override GameObject RestoreToDefault() { return gameObject; }

    public override void ReturnToPool() { ResourceTowerPool.Instance.ReturnResourceTower(gameObject); }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
