using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.InputSystem;

public class TutorialController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tutorialCanvas;          // Panel del tutorial
    public TextMeshProUGUI tutorialMessageText; // Texto del mensaje del tutorial
    public TextMeshProUGUI instructionText;     // Texto fijo: "Presiona Enter para continuar"

    [Header("Tutorial Data")]
    public List<TutorialModule> tutorialModules; // Lista de módulos en el Inspector
    private int currentModuleIndex = 0;         // Índice del módulo actual
    private int currentMessageIndex = 0;        // Índice del mensaje dentro del módulo
    private bool isMessageActive = false;       // Indica si hay un mensaje en pantalla

    [Header("Input")]
    public InputActionReference continueAction; // Acción para avanzar

    private void OnEnable()
    {
        continueAction.action.Enable();
        continueAction.action.performed += OnContinueAction;
    }

    private void OnDisable()
    {
        continueAction.action.performed -= OnContinueAction;
        continueAction.action.Disable();
    }

    private void Start()
    {
        tutorialCanvas.SetActive(false); // Asegurar que el canvas está oculto al inicio
        StartCoroutine(LoadInstruction()); // Cargar el texto fijo de "Presiona Enter para continuar"
    }

    private IEnumerator LoadInstruction()
    {
        var localizedString = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TutorialTable", "InstructionKey");
        yield return localizedString;
        instructionText.text = localizedString.Result;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateModule();
        }
    }

    private void ActivateModule()
    {
        if (currentModuleIndex < tutorialModules.Count)
        {
            PauseGame();
            ShowMessage();
        }
    }

    private void ShowMessage()
    {
        if (currentModuleIndex < tutorialModules.Count)
        {
            var module = tutorialModules[currentModuleIndex];

            if (currentMessageIndex < module.messages.Count)
            {
                StartCoroutine(DisplayLocalizedMessage(module.messages[currentMessageIndex]));
                currentMessageIndex++;
            }
            else
            {
                currentMessageIndex = 0;
                currentModuleIndex++;
                ResumeGame();
            }
        }
    }

    private IEnumerator DisplayLocalizedMessage(LocalizedString key)
    {
        var localizedString = key.GetLocalizedStringAsync();
        yield return localizedString;

        tutorialMessageText.text = localizedString.Result;
        tutorialCanvas.SetActive(true);
        isMessageActive = true;
    }

    private void OnContinueAction(InputAction.CallbackContext ctx)
    {
        if (isMessageActive)
        {
            tutorialCanvas.SetActive(false);
            isMessageActive = false;
            ShowMessage();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}

[System.Serializable]
public class TutorialModule
{
    public string moduleName;             // Nombre del módulo (visible en el Inspector)
    public List<LocalizedString> messages; // Lista de mensajes localizados
}

    /*[Header("UI Elements")]
    public GameObject TutorialCanvas;   //Ventana flotante con info del tutorial
    public TextMeshProUGUI tutorialMessageText;//Texto del mensaje del tutorial
    public TextMeshProUGUI instructionText;    //Texto fijo
    //public Image unlockedItemIcon;      //Imagen del icono desbloqueado
    //public GameObject iconHighlightFrame;//Marco iluminado que identifica el elemento destacado

    [Header("Localization")]
    public string localizationTableName;    //Nombre de la tabla de localizacion del juego
    public string[] messageKeys;            //Claves de los mensajes en el orden en el que deben mostrarse
    public string instructionKey;           // Clave del mensaje de instrucci�n fijo

    private int currentMessageIndex = 0;        //Indice del mensaje actual
    private bool isMessageActive = false;   //Indicador de mensajes activos

    [Header("Input")]
    public InputActionReference continueAction; //Accion de continuar

    private void OnEnable()
    {
        //Vincular el evento de entrada
        continueAction.action.Enable();
        continueAction.action.performed += OnContinueAction;
    }

    private void OnDisable()
    {
        continueAction.action.performed -= OnContinueAction;
        continueAction.action.Disable();
    }

    // Start is called before the first frame update
    private void Start()
    {
        //Comprobar que todo esta desactivado al inicio
        TutorialCanvas.SetActive(false);
        //iconHighlightFrame.SetActive(false);
        //unlockedItemIcon.gameObject.SetActive(false);
        //Cargar texto fijo de instruccion
        StartCoroutine(LoadInstruction());
        //Mostrar el siguiente mensaje del tutorial
        ShowNextMessage();
    }

    private IEnumerator LoadInstruction()
    {
        //Cargar texto fijo de instruccion
        var localizedString = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(localizationTableName, instructionKey);
        yield return localizedString;

        //Configurar texto
        instructionText.text = localizedString.Result;
    }

    public void ShowNextMessage() 
    {
        if (currentMessageIndex < messageKeys.Length)
        {
            PauseTutorial();
            StartCoroutine(DisplayLocalizedMessage(messageKeys[currentMessageIndex]));
            currentMessageIndex++;
        }
        else
        {
            ResumeTutorial();
        }
    }

    private IEnumerator DisplayLocalizedMessage(string key)
    {
        //Cargar el mensaje localizado
        var localizedString = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(localizationTableName, key);
        yield return localizedString;

        //Mostrar el mensaje en la UI
        tutorialMessageText.text = localizedString.Result;

        //Activar la ventana modal
        TutorialCanvas.SetActive(true);
        isMessageActive = true;

        //Si tiene un icono relacionado, activarlo y resaltarlo
        //if (unlockedItemIcon != null && iconHighlightFrame != null)
        {
            unlockedItemIcon.gameObject.SetActive(true);
            iconHighlightFrame.SetActive(true);
        }//
    }

    private void OnContinueAction(InputAction.CallbackContext ctx)
    {
        if (isMessageActive)
        {
            //Desactivar la ventana modal y sus elementos
            TutorialCanvas.SetActive(false);
            //iconHighlightFrame.SetActive(false);
            //unlockedItemIcon.gameObject.SetActive(false);

            isMessageActive = false;

            //Mostrar el siguiente mensaje
            ShowNextMessage();
        }
    }

    private void PauseTutorial()
    {
        Time.timeScale = 0f; //Pausa el paso del tiempo en el juego
    }

    private void ResumeTutorial()
    {
        Time.timeScale = 1f; // Reestablece el paso del tiempo en el juego
    }*/

