using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    [SerializeField]
    GameObject currentSwitchable;
    [SerializeField]
    GameObject targetSwitchable;

    public void Switch()
    {
        
        if (currentSwitchable.TryGetComponent<SwitchStates.ISwitchable>(out var current))
        {
            current.Deactivate();
        }

        
        if (targetSwitchable.TryGetComponent<SwitchStates.ISwitchable>(out var target))
        {
            target.Activate();
        }
    }
}
