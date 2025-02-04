using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{
    //Asegura que el trigger se activa solo una vez
    private bool hasActivated = false;
    public UnityEvent pachamamaEvent, enemiesEvent, tutorialEvent;

    public UIButtonsController fadeController;
    public float fadeDuration = 1f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasActivated)
        {
            hasActivated = true;
            TutorialController.Instance.ActivateModule();
            pachamamaEvent.Invoke();
            tutorialEvent.Invoke();

            if (fadeController != null)
            {
                //fadeController.FadeInButton(fadeDuration);
            }

            Destroy(gameObject);
        }
        if (other.CompareTag("Enemy"))
        {
            enemiesEvent.Invoke();
        }
    }
}
