using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ISwitchStates;

public class SwitchController : MonoBehaviour
{
    [SerializeField]
    GameObject currentSwitchable;

    [SerializeField]
    GameObject targetSwitchable;

    public static bool timeToChange;

    private void Start()
    {
        timeToChange = false;
    }

    public void Switch()
    {
        if (timeToChange)
        {
            targetSwitchable.SetActive(true);
            currentSwitchable.SetActive(false);
        }
    }
}
