using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }
    public GameObject buildUI;
    public GameObject crossHead;

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
}
