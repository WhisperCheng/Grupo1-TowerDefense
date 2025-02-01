using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        languageCanvas.GetComponent<CanvasGroup>();
        languageCanvas.SetActive(false);
    }
    
    private void BajarAlpha()
    {    
        LeanTween.alpha(introCanvas.GetComponent<RectTransform>(), 0f, 1f).setDelay(0.75f);
        introCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
        languageCanvas.SetActive(true);
    }
}

