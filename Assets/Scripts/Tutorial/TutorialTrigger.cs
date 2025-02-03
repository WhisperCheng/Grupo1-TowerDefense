using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{
    //Asegura que el trigger se activa solo una vez
    private bool hasActivated = false;
    public UnityEvent pachamamaEvent, enemiesEvent;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasActivated)
        {
            Debug.Log("Estas reconociendo el contacto" + gameObject.name);
            hasActivated = true;
            TutorialController.Instance.ActivateModule();
            pachamamaEvent.Invoke();
            Destroy(gameObject);
        }
        if (other.CompareTag("Enemy"))
        {
            enemiesEvent.Invoke();
        }
    }
}
