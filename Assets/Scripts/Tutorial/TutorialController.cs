using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.InputSystem;
using System.ComponentModel;
using UnityEditor.VersionControl;

public class TutorialController : MonoBehaviour
{
    //Singleton: Hace el script accesible
    public static TutorialController Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject tutorialCanvas;          // Panel del tutorial
    public TextMeshProUGUI tutorialMessageText; // Texto del mensaje del tutorial
    public TextMeshProUGUI instructionText;     // Texto fijo: "Presiona Enter para continuar"
    public RectTransform arrowImage;

    [Header("Tutorial Data")]
    public List<TutorialModule> tutorialModules; // Lista de módulos en el Inspector
    private int currentModuleIndex = 0;         // Índice del módulo actual
    private int currentMessageIndex = 0;        // Índice del mensaje dentro del módulo
    private bool isMessageActive = false;       // Indica si hay un mensaje en pantalla

    [Header("Input")]
    public InputActionReference continueAction; // Acción para avanzar

    private void OnEnable()
    {
        //Habilita la deteccion de la tecla Continuar
        continueAction.action.Enable();
        continueAction.action.performed += OnContinueAction;
    }

    private void OnDisable()
    {
        //Desabilita la accion cuando el objeto de desactiva
        continueAction.action.performed -= OnContinueAction;
        continueAction.action.Disable();
    }

    private void Awake()
    {
        //Implementar singleton para evitar duplicados
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        tutorialCanvas.SetActive(false);    // Asegurar que el canvas está oculto al inicio
        arrowImage.gameObject.SetActive(false);
        StartCoroutine(LoadInstruction());  // Cargar el texto fijo de "Presiona Enter para continuar"
    }

    //Carga de la instruccion de continue
    private IEnumerator LoadInstruction()
    {
        var localizedString = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Localization Table", "_PresionarGTutoText");
        yield return localizedString;
        instructionText.text = localizedString.Result;
    }

    //Activa el modulo de tutorial (Contenedor de mensajes) cuando el jugador entra en el trigger designado
    public void ActivateModule()
    {
        if (currentModuleIndex < tutorialModules.Count)
        {
            PauseGame();                //Pausa el juego
            currentMessageIndex = 0;    //Reinicia la secuencia de mensajes
            StartMessages();              //Muestra el mensaje que corresponde
        }
    }

    // Muestra un mensaje del módulo actual. Si ya se mostraron todos los mensajes, avanza al siguiente módulo.
    private void StartMessages()
    {
        if (currentModuleIndex < tutorialModules.Count)
        {
            TutorialModule module = tutorialModules[currentModuleIndex]; //Obtener módulo actual

            if (currentMessageIndex < module.messages.Count)
            {
                TutorialMessage message = module.messages[currentMessageIndex];
                //Mostrar el mensaje actual y avanzar al siguiente
                StartCoroutine(DisplayLocalizedMessage(message));
                currentMessageIndex++; // Avanzar al siguiente mensaje dentro del módulo
            }
            else
            {
                EndModule(); // Aquí solo entraría en el caso de que no hubieran mensajes en el módulo, en teoría
            }
        }
    }

    // Obtiene el texto localizado del mensaje y lo muestra en pantalla.
    private IEnumerator DisplayLocalizedMessage(TutorialMessage message)
    {
        var localizedString = message.messageText.GetLocalizedStringAsync();
        yield return localizedString;

        tutorialCanvas.SetActive(true); // Hay que mostrar el canvas para que pueda cambiar el texto
        isMessageActive = true;
        tutorialMessageText.text = localizedString.Result;

        if (message.showArrow)
        {
            arrowImage.gameObject.SetActive(true);
            arrowImage.anchoredPosition = message.arrowPosition;
            arrowImage.rotation = Quaternion.Euler(0, 0, message.arrowRotation);
        }
        else
        {
            arrowImage.gameObject.SetActive(false);
        }
    }

    // Maneja la acción de "continuar" cuando el jugador presiona la tecla.
    private void OnContinueAction(InputAction.CallbackContext ctx)
    {
        if (isMessageActive && ctx.performed)
        {
            if (currentMessageIndex < tutorialModules[currentModuleIndex].messages.Count)
            {
                
                StartMessages();
            }
            else
            {
                EndModule();
            }
            isMessageActive = false;
        }
    }

    //Terminar el modulo actual y pasar al siguiente
    private void EndModule() 
    {
        tutorialCanvas.SetActive(false);
        arrowImage.gameObject.SetActive(false);
        ResumeGame();
        currentModuleIndex++;
        currentMessageIndex = 0;
    }

    //Pausar juego mientras se muestran mensajes de tutorial
    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    //Reanudar juego despues de interactuar con el tutorial
    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
[System.Serializable]
public class TutorialMessage
{
    public LocalizedString messageText;   // Texto del mensaje
    public bool showArrow;                // Indica si la flecha debe mostrarse
    public Vector2 arrowPosition;         // Posición de la flecha en pantalla
    public float arrowRotation;           // Rotación de la flecha
}

[System.Serializable]
public class TutorialModule
{
    public string moduleName;               // Nombre del módulo (visible en el Inspector)
    public List<TutorialMessage> messages;  // Lista de mensajes y sus configuraciones
}