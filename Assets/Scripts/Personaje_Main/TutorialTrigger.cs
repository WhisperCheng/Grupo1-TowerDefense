using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    //Asegura que el trigger se activa solo una vez
    private bool hasActivated = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasActivated)
        {
            Debug.Log("Estas reconociendo el contacto" + gameObject.name);
            hasActivated = true;
            TutorialController.instance.ActivateModule();
            Destroy(gameObject);
        }
    }
}
