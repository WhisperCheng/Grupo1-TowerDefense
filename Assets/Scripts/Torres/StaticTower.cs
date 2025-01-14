using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StaticTower : Tower
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, towerRadiusSize);
    }
}
