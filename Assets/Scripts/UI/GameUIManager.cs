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
    public GameObject[] panelsMenu; 
    public TMP_Text textMoney;
    
    private string textMoneyOriginal;

    public bool activeMenuPause;
    public bool activeBuildUI;
    bool otherPanelActive;
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
            if (activeMenuPause && !otherPanelActive)
            {
                OpenGameMenu();
            }
            else if (!otherPanelActive)
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
        LeanTween.moveY(buildUI, -80f, time).setEaseInOutSine(); // Mostrar menú de botones
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
        activeMenuPause = true;
        menuPause.SetActive(true);
        Time.timeScale = 0f;
    }
    void CloseGameMenu()
    {
        //añadir leAntween y esa vaina loca
        activeMenuPause = false;
        menuPause.SetActive(false);
        Time.timeScale = 1f;
    }
    void GamePostProcessingMenu()
    {
        //Hacer que se desenfoque el fondo con Post processing > Depth Of Field > Focal Length
        //Bajar/subir el sistema de rondas y los botones de abajo
    }
    void GameMenuDesactivate()
    {
        Time.timeScale = 0f;
        otherPanelActive = true;
    }
    public void SoundButton()
    {
        GameMenuDesactivate();
        panelsMenu[0].SetActive(true);
    }
    public void BrightnessButton()
    {
        GameMenuDesactivate();
        panelsMenu[1].SetActive(true);
    }
    public void ControlButton()
    {
        GameMenuDesactivate();
        panelsMenu[2].SetActive(true);
    }
    public void StartGame()
    {
        CloseGameMenu();
    }
    public void BackToPrincipalMenu()
    {
        SceneManager.LoadScene("MainTutorial");
    }
    public void AcceptButton()
    {
        foreach (var panels in panelsMenu)
        {
            panels.SetActive(false);
            otherPanelActive = false;
            OpenGameMenu();
        }
    }
}
