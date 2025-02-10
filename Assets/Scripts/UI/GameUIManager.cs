using UnityEngine;
using UnityEngine.InputSystem;
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

    [Header("Parámetros UI Juego")]
    public float hideToolbarCoordinates;
    public float showToolbarCoordinates;

    public bool activeMenuPause;
    public bool activeBuildUI;
    public bool otherPanelActive;
    public float menusTransitionTime = 0.5f;

    public Vector3 CanvasProportion { get; private set; }

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

    void Start()
    {
        textMoneyOriginal = textMoney.text;

        Cursor.visible = false; // Ocultar cursor y lockearlo al centro
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CanvasProportion = mainCanvas.localScale; // Proporción de la escala del canvas
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
                previousTimeScale = Time.timeScale; // Este tiene que ir aquí
                OpenPauseMenu();
            }
            else if (!otherPanelActive)
            {
                ClosePauseMenu();
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

        // Esconder/Mostrar menú de botones con movimiento relativo a la escala del canvas
        LeanTween.moveY(buildUI, verticalMovement * CanvasProportion.y, time).setEaseInOutSine();
    }

    public void OpenPauseMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        activeMenuPause = true;
        menuPause.SetActive(true);
        buildUI.SetActive(false);
        roundUI.SetActive(false);
        PostProcessingControl.Instance.PostProcessingVolumeOn();
        Time.timeScale = 0f;
        hotBarController.DisableHotbar();
    }
    public void ClosePauseMenu()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        activeMenuPause = false;
        menuPause.SetActive(false);
        buildUI.SetActive(true);
        roundUI.SetActive(true);
        PostProcessingControl.Instance.PostProcessingVolumeOff();
        Time.timeScale = previousTimeScale;
        hotBarController.EnableHotbar();
    }
    private void GameMenuDesactivate()
    {
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void BackToPrincipalMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
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
        Cursor.visible = true;
        panelWin.SetActive(true);
        GameMenuDesactivate();
    }
    public void LoseLevel()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        panelLose.SetActive(true);
        GameMenuDesactivate();
    }
}
