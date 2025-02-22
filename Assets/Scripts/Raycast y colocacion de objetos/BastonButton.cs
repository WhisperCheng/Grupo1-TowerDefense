using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BastonButton : MonoBehaviour
{
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {

    }

    public void SetHoldingCrosier(Player player)
    {
        if (player.CheckIfIsShowingCrosier()) // Si no est� mostrando el bast�n lo baja
        {
            EventSystem.current.SetSelectedGameObject(null); // Deseleccionar bot�n
            player.HideCrosier();
        }
        else // Si lo est� escondiendo lo sube
        {
            player.ShowCrosier();
        }
    }

    public void HideCrosier(Player player)
    {
        EventSystem.current.SetSelectedGameObject(null); // Deseleccionar bot�n
        player.HideCrosier();
    }

    public void ShowCrosier(Player player)
    {
        player.ShowCrosier();
    }
}
