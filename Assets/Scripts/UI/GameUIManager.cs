using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    public PlayerInput playerInput;
    public GameObject objectUI;
    public GameObject buttonsBar;
    public GameObject crossHead;

    public bool activeObjectUI;
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

        if (activeObjectUI)
        {
            showObjectMenu(0);
        }
        else
        {
            hideObjectMenu(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onToggleObjectUI(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //float transitionTime = 1f;
            if (activeObjectUI)
            {
                showObjectMenu(menusTransitionTime);
            }
            else
            {
                hideObjectMenu(menusTransitionTime);
            }
        }
        activeObjectUI = !activeObjectUI;
    }

    public void hideObjectMenu(float time)
    {
        activeObjectUI = false; // Ocultar crosshead
        crossHead.SetActive(false); //
        LeanTween.cancel(objectUI); // reset de las animaciones
        LeanTween.cancel(buttonsBar); //
        LeanTween.moveY(objectUI, -70f, time).setEaseInOutSine(); // Mostrar menú de botones
        LeanTween.moveLocalY(buttonsBar, -74f, time).setEaseInOutSine(); // 
    }

    public void showObjectMenu(float time)
    {
        activeObjectUI = true; // Ocultar crosshead
        crossHead.SetActive(true); //
        LeanTween.cancel(objectUI); // reset de las animaciones
        LeanTween.cancel(buttonsBar); // 
        LeanTween.moveY(objectUI, 40.5f, time).setEaseInOutSine(); // Mostrar menú de botones
        LeanTween.moveLocalY(buttonsBar, 0f, time).setEaseInOutSine(); //
    }
}
