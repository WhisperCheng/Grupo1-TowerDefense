using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchController : MonoBehaviour
{
    [SerializeField] private GameObject currentSwitchable; 
    [SerializeField] private GameObject targetSwitchable;  
    [SerializeField] private float delay = 0.6f;           


    public void  Switch()
    {
           StartCoroutine(SwitchAfterAnimation(delay));

           AudioManager.instance.PlayOneShot(FMODEvents.instance.menuClick, this.transform.position);

    }

    private IEnumerator SwitchAfterAnimation(float delay)
    {
            yield return new WaitForSeconds(delay);   
            targetSwitchable.SetActive(true);
            currentSwitchable.SetActive(false);

       // AudioManager.instance.InitializeMusic(FMODEvents.instance.musicMenu);
    }
}

