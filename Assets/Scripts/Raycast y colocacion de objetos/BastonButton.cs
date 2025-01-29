using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BastonButton : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }
    
    public void SetHoldingCrosier(Player player)
    {
        //if (EventSystem.current.currentSelectedGameObject == gameObject)
        if (player.CheckIfIsShowingCrosier()) // Si no est� mostrando el bast�n lo baja
        {
            EventSystem.current.SetSelectedGameObject(null); // Deseleccionar bot�n
            player.HideCrosier();
        }
        else // Si lo est� escondiendo lo sube
        {
            button.Select();
            player.ShowCrosier();
        }
    }
}
