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
    [SerializeField]
    private float delay = 0.6f;
    [SerializeField] Vector2 finalPos;

    public void Switch()
    {
        StartCoroutine(SwitchAfterAnimation(delay));
        MenuUIManager.Instance.ExecuteAnimation(gameObject, delay, finalPos);

    }
    private IEnumerator SwitchAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        targetSwitchable.SetActive(true);
        currentSwitchable.SetActive(false);
    }
}
