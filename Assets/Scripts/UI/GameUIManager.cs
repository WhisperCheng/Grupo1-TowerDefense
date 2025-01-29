using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }
    public GameObject buildUI;
    public GameObject crossHead;
    public GameObject menuPause;
    public GameObject menuSound;
    public GameObject menuBrightness;
    public GameObject menuControl;
    public TMP_Text textMoney;
    
    private string textMoneyOriginal;

    public bool activeMenuPause;
    public bool activeBuildUI;
    public float menusTransitionTime = 0.5f;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //activeObjectUI = true;
        textMoneyOriginal = textMoney.text;
        if (activeBuildUI)
        {
            ShowBuildUI(0);
        }
        else
        {
            HideBuildUI(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMoney();
    }

    public void OnToggleBuildUI(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //float transitionTime = 1f;
            if (activeBuildUI)
            {
                ShowBuildUI(menusTransitionTime);
            }
            else
            {
                HideBuildUI(menusTransitionTime);
            }
        }
        activeBuildUI = !activeBuildUI;
    }
    public void OnClickGameMenu (InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (activeMenuPause)
            {
                OpenGameMenu();
            }
            else
            {
                CloseGameMenu();
            }
        }
        activeMenuPause = !activeMenuPause;
    }
    public void HideBuildUI(float time)
    {
        activeBuildUI = false; // Ocultar crosshead
        crossHead.SetActive(false); //
        LeanTween.cancel(buildUI); // reset de las animaciones
        LeanTween.moveY(buildUI, -70f, time).setEaseInOutSine(); // Mostrar menú de botones
        //LeanTween.moveLocalY(buildButtons, -74f, time).setEaseInOutSine(); // 
    }

    public void ShowBuildUI(float time)
    {
        activeBuildUI = true; // Ocultar crosshead
        crossHead.SetActive(true); //
        LeanTween.cancel(buildUI); // reset de las animaciones
        LeanTween.moveY(buildUI, 40.5f, time).setEaseInOutSine(); // Mostrar menú de botones
        //LeanTween.moveLocalY(buildButtons, 0f, time).setEaseInOutSine(); //
    }

    public void UpdateMoney()
    {
        textMoney.text = textMoneyOriginal + " " + MoneyManager.Instance.GetMoney();
    }

    void OpenGameMenu()
    {
        //añadir leAntween y esa vaina loquísima
        //Hacer que se desenfoque el fondo con Post processing > Depth Of Field > Focal Length
        //Bajar/subir el sistema de rondas y los botones de abajo
        activeMenuPause = true;
        menuPause.SetActive(true);
        Time.timeScale = 0f;
    }
    void CloseGameMenu()
    {
        //añadir leAntween y esa vaina loca
        if (activeMenuPause == true)
        {
            activeMenuPause = false;
            menuPause.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    void GameMenuDesactivate()
    {
        menuPause.SetActive(false);

    }
    public void SoundButton()
    {
        menuPause.SetActive(false);
        menuSound.SetActive(true);
    }
    public void BrightnessButton()
    {
        menuPause.SetActive(false);
        menuBrightness.SetActive(true);
    }
    public void ControlButton()
    {
        menuPause.SetActive(false);
        menuControl.SetActive(true);
    }
    public void ReStartGame()
    {
        CloseGameMenu();
    }
    public void BackToPrincipalMenu()
    {
        SceneManager.LoadScene("MainTutorial");
    }
    public void AcceptButton()
    {
        menuSound.SetActive(false);
        menuBrightness.SetActive(false);
        menuControl.SetActive(false);
        activeMenuPause = true;
    }
}
