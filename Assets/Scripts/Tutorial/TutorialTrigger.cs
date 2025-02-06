using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    //Asegura que el trigger se activa solo una vez
    private bool hasActivated = false;
    public UnityEvent pachamamaEvent, enemiesEvent;
    public List<Image> elementsActivated;
    public float fadeDuration = 1f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasActivated)
        {
            hasActivated = true;
            TutorialController.Instance.ActivateModule();
            pachamamaEvent.Invoke();
            //StartCoroutine(FadeInButtons());
            Destroy(gameObject);
        }

        if (other.CompareTag("Enemy"))
        {
            enemiesEvent.Invoke();
        }
    }
}
