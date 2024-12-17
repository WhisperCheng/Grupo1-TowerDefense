using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    [Header("Intro Elements")]
    [SerializeField] private GameObject selectLanguageCanvas;

    [Header("Main Menu Elements")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject selectLevelCanvas;
    [SerializeField] private GameObject optionsMenuCanvas;
    [SerializeField] private GameObject creditsMenuCanvas;

    [Header("Options Menu Elements")]
    [SerializeField] private GameObject brightnessOptionsCanvas;
    [SerializeField] private GameObject soundOptionsCanvas;
    [SerializeField] private GameObject controlsCanvas;

    [Header("Animation Parameters")]
    [SerializeField] private LeanTweenType easeInScale;
    [SerializeField] private float animationInTime;

    public void OpenMainMenu()
    {
        LeanTween.scale(selectLanguageCanvas, Vector3.zero, animationInTime);
        mainMenuCanvas.SetActive(true);
        LeanTween.scale(mainMenuCanvas, Vector3.one, animationInTime).setEase(easeInScale);
    }
}
