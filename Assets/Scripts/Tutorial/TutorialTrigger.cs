using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    //Asegura que el trigger se activa solo una vez
    private bool hasActivated = false;
    public UnityEvent pachamamaEvent;
    public List<Image> elementsActivated;
    /*public GameObject wavesActivator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (wavesActivator != null)
            {
                wavesActivator.SetActive(true);
            }
        }
    }*/
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasActivated)
        {
            hasActivated = true;
            TutorialController.Instance.ActivateModule();
            pachamamaEvent.Invoke();
            Destroy(gameObject);
        }
    }
}
