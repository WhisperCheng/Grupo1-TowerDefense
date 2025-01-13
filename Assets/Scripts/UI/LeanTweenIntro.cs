using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeanTweenIntro : MonoBehaviour
{
    [Header("Intro Elements")]
    [SerializeField]
    GameObject logoImage;
    [SerializeField]
    GameObject introCanvas;
    [SerializeField]
    GameObject languageCanvas;

    void Start()
    {
        LeanTween.moveY(logoImage.GetComponent<RectTransform>(), 0, 2.7f).setDelay(0.5f).setEase(LeanTweenType.easeInOutBack).setOnComplete(BajarAlpha);


    }
    
    private void BajarAlpha()
    {
       
        LeanTween.alpha(introCanvas.GetComponent<RectTransform>(), 0f, 1f).setDelay(0.75f).setOnComplete(ActivateTheGameMenu);
        introCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    private void ActivateTheGameMenu()
    {
        SwitchController.timeToChange = true;
    }
}

