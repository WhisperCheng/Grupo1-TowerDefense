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
        LeanTween.moveY(logoImage.GetComponent<RectTransform>(), 0, 2.7f).setDelay(0.5f).setEase(LeanTweenType.easeInOutBack);


    }
  private void BajarAlpha()
    {

        LeanTween.alpha(introCanvas.GetComponent<RectTransform>(), 0f, 1f).setDelay(1f);
        introCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}
}
