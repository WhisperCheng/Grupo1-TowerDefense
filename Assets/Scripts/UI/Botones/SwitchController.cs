using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ISwitchStates;
using FMODUnity;

public class SwitchController : MonoBehaviour
{
    [SerializeField]
    GameObject currentSwitchable;
    [SerializeField]
    GameObject targetSwitchable;
    public static bool timeToChange;
    [SerializeField]
    private float delay = 0.6f;
    [SerializeField] Vector2 finalPos;
    [SerializeField] private LeanTweenType animationButton;

    public void Switch()
    {
        StartCoroutine(SwitchAfterAnimation(delay));
        MenuUIManager.Instance.ExecuteAnimation(gameObject, delay, finalPos, animationButton);

        //AudioManager.instance.PlayOneShot(FMODEvents.instance.menuClick, this.transform.position);

    }


    private IEnumerator SwitchAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        targetSwitchable.SetActive(true);
        currentSwitchable.SetActive(false);
    }
}
