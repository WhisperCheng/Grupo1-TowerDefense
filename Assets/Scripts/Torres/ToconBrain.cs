using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToconBrain : MonoBehaviour
{
    private GameObject _currentTarget;
    private float _spawnCooldown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetSpawnCooldown(float time)
    {
        _spawnCooldown = time;
    }
    public void SetCurrentTarget(GameObject target)
    {
        _currentTarget = target;
    }

    public GameObject GetCurrentTarget()
    {
        return _currentTarget;
    }
}
