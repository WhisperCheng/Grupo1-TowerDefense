using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeanTweenIntro : MonoBehaviour
{

    [SerializeField]
    GameObject logoImage;
    [SerializeField]
    GameObject introCanvas;

    void Start()
    {
        LeanTween.moveY(logoImage.GetComponent<RectTransform>(), 0, 2.7f).setDelay(0.5f).setEase(LeanTweenType.easeInOutBack).setOnComplete(BajarAlpha);


    }
  private void BajarAlpha()
    {
       
        LeanTween.alpha(introCanvas.GetComponent<RectTransform>(), 0f, 1f).setDelay(0.75f);
        introCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;

    }
}

