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
    [Header("Paneles y componentes de la UI")]
    public RectTransform mainCanvas;
    public GameObject buildUI;
    public GameObject roundUI;
    public GameObject crossHead;
    public GameObject panelWin;
    public GameObject panelLose;
    public GameObject menuPause;
    public HotbarController hotBarController;
    public GameObject[] panelsMenu;
    public TMP_Text textMoney;

    [Header("Par�metros UI Juego")]
    public float hideToolbarCoordinates;
    public float showToolbarCoordinates;

    public bool activeMenuPause;
    public bool activeBuildUI;
    public bool otherPanelActive;
    public float menusTransitionTime = 0.5f;

    public Vector3 CanvasProportion { get; private set; }

    private Vector3 bottomHotbarPos;
    private RectTransform canvasRT;

    private float previousTimeScale = 1;

    private string textMoneyOriginal;

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
        canvasRT = mainCanvas.GetComponent<RectTransform>();

        textMoneyOriginal = textMoney.text;
    }

    // Update is called once per frame
    void Update()
    {
        CanvasProportion = mainCanvas.localScale; // Proporci�n de la escala del canvas
        UpdateMoney();
    }
    public void UpdateMoney()
    {
        textMoney.text = textMoneyOriginal + " " + MoneyManager.Instance.GetMoney();
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
    public void OnClickGameMenu(InputAction.CallbackContext ctx) // Al presionar Esc
    {
        if (ctx.performed)
        {
            if (activeMenuPause && !otherPanelActive)
            {
                previousTimeScale = Time.timeScale; // Este tiene que ir aqu�
                OpenPauseMenu();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (!otherPanelActive)
            {
                ClosePauseMenu();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                //Time.timeScale = previousTimeScale;
            }
        }
        activeMenuPause = !activeMenuPause;
    }
    public void HideBuildUI(float time)
    {
        ToggleBuildUI(false, hideToolbarCoordinates, time);
    }
    public void ShowBuildUI(float time)
    {
        ToggleBuildUI(true, showToolbarCoordinates, time);
    }
    private void ToggleBuildUI(bool value, float verticalMovement, float time)
    {
        activeBuildUI = value; // Alternar crosshead
        crossHead.SetActive(value);
        LeanTween.cancel(buildUI); // Reset de las animaciones

        // Esconder/Mostrar men� de botones con movimiento relativo a la escala del canvas

        //bottomHotbarPos = canvasRT.TransformDirection(new Vector2(0, canvasRT.rect.yMin));
        //LeanTween.moveLocalY(buildUI, bottomHotbarPos.y + verticalMovement, time).setEaseInOutSine();
        // Se puede de esta otra manera, pero la segunda da menos problemas
        LeanTween.moveY(buildUI, verticalMovement * CanvasProportion.y, time).setEaseInOutSine();
    }

    public void OpenPauseMenu()
    {
        //a�adir leAntween y esa vaina loqu�sima
        activeMenuPause = true;
        menuPause.SetActive(true);
        buildUI.SetActive(false);
        roundUI.SetActive(false);
        PostProcessingControl.Instance.PostProcessingVolumeOn();
        //previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        hotBarController.DisableHotbar();
    }
    public void ClosePauseMenu()
    {
        //a�adir leAntween y esa vaina loca
        activeMenuPause = false;
        menuPause.SetActive(false);
        buildUI.SetActive(true);
        roundUI.SetActive(true);
        PostProcessingControl.Instance.PostProcessingVolumeOff();
        //Time.timeScale = 1f;
        Time.timeScale = previousTimeScale;
        hotBarController.EnableHotbar();
    }
    private void GameMenuDesactivate()
    {
        //previousTimeScale = Time.timeScale;
        //Time.timeScale = 0f;
        otherPanelActive = true;
    }
    public void ControlButton()
    {
        GameMenuDesactivate();
        panelsMenu[0].SetActive(true);
    }
    public void SoundButton()
    {
        GameMenuDesactivate();
        panelsMenu[1].SetActive(true);
    }
    public void BrightnessButton()
    {
        GameMenuDesactivate();
        panelsMenu[2].SetActive(true);
    }

    public void StartGame()
    {
        ClosePauseMenu();
    }
    public void ReLoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
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
            OpenPauseMenu();
        }
    }
    public void WinLevel()
    {
        Cursor.lockState = CursorLockMode.None;
        panelWin.SetActive(true);
        GameMenuDesactivate();
    }
    public void LoseLevel()
    {
        Cursor.lockState = CursorLockMode.None;
        panelLose.SetActive(true);
        GameMenuDesactivate();
    }
}
