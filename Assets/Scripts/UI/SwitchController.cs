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
    [SerializeField]
    public MenuUIManager menuUIManager;

    public static bool timeToChange;

    private void Start()
    {
        timeToChange = false;
    }

    public void Switch()
    {
        StartCoroutine(SwitchAfterAnimation(menuUIManager.animationTimer));
       
    }
     private IEnumerator SwitchAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay); 
        targetSwitchable.SetActive(true);
        currentSwitchable.SetActive(false);
    }
}
