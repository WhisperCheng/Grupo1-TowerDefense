using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.InputSystem;
using System.ComponentModel;

public class TutorialController : MonoBehaviour
{
    //Singleton: Hace el script accesible
    public static TutorialController instance { get; private set; }

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
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        tutorialCanvas.SetActive(false);    // Asegurar que el canvas está oculto al inicio
        StartCoroutine(LoadInstruction());  // Cargar el texto fijo de "Presiona Enter para continuar"
    }

    //Carga de la instruccion de continue
    private IEnumerator LoadInstruction()
    {
        var localizedString = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TutorialTable", "InstructionKey");
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
            ShowMessage();              //Muestra el mensaje que corresponde
        }
    }

    // Muestra un mensaje del módulo actual. Si ya se mostraron todos los mensajes, avanza al siguiente módulo.
    private void ShowMessage()
    {
        if (currentModuleIndex < tutorialModules.Count)
        {
            var module = tutorialModules[currentModuleIndex];   //Obtener módulo actual

            if (currentMessageIndex < module.messages.Count)
            {
                //Mostrar el mensaje actual y avanzar al siguiente
                StartCoroutine(DisplayLocalizedMessage(module.messages[currentMessageIndex]));
                //currentMessageIndex++;
            }
            else
            {
                //Si se mostraron todos los mensajes, avanzar al siguiente modulo
                currentMessageIndex = 0;
                currentModuleIndex++;
                ResumeGame(); //Regresar al juego
            }
        }
    }

    // Obtiene el texto localizado del mensaje y lo muestra en pantalla.
    private IEnumerator DisplayLocalizedMessage(LocalizedString key)
    {
        var localizedString = key.GetLocalizedStringAsync();
        yield return localizedString;

        tutorialMessageText.text = localizedString.Result;
        tutorialCanvas.SetActive(true);
        isMessageActive = true;
    }

    // Maneja la acción de "continuar" cuando el jugador presiona la tecla.
    private void OnContinueAction(InputAction.CallbackContext ctx)
    {
        if (isMessageActive)
        {
            tutorialCanvas.SetActive(false);
            isMessageActive = false;
            currentMessageIndex++; // Avanzar al siguiente mensaje dentro del módulo
            ShowMessage();
        }
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

// Clase para almacenar los datos de cada módulo del tutorial.
[System.Serializable]
public class TutorialModule
{
    public string moduleName;             // Nombre del módulo (visible en el Inspector)
    public List<LocalizedString> messages; // Lista de mensajes localizados
}