using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TowerInteractionManager : MonoBehaviour
{
    [Header("Porcentaje de gemas recibidas al vender torre")]
    [Range(0, 100)]
    public float sellingPercentageAmount;

    [Header("Tiempo de esperas")]
    public float sellingTowerTime;
    public float upgradingTowerTime;

    public Image spriteTowerSelling;
    public Image spriteTowerBoosting;

    private bool sellingButtonPressed;
    private bool boostingButtonPressed;
    private bool canBoostCurrentTower;

    public static TowerInteractionManager Instance { get; private set; }
    private void Awake()
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

    public void SellTower(InputAction.CallbackContext ctx)
    {
        spriteTowerSelling.fillAmount = 0;
        if (!boostingButtonPressed)
            sellingButtonPressed = ButtonPressed(ctx, spriteTowerSelling); // To keep the state in a boolean. 
        // Cambia sellingButtonPressed a true si está siendo presionado, y a false si deja de estarlo
    }

    public void BoostTower(InputAction.CallbackContext ctx)
    {
        spriteTowerBoosting.fillAmount = 0;
        if (ctx.started && !ctx.performed) // Cuando se presiona la tecla
        {   // Se comprueba si se puede boostear la torre a la que se está apuntando
            canBoostCurrentTower = CanBoostCurrentTower();
        }

        if (!sellingButtonPressed && canBoostCurrentTower)
            boostingButtonPressed = ButtonPressed(ctx, spriteTowerBoosting); // To keep the state in a boolean. 
        // Cambia boostingButtonPressed a true si está siendo presionado, y a false si deja de estarlo,
        // y solo si se puede boostear la torre a la que se está apuntando
    }

    private bool ButtonPressed(InputAction.CallbackContext ctx, Image spriteImage)
    {
        // To keep the state in a boolean. // Cambia sellingButtonPressed a true si está siendo presionado,
        bool buttonPressed = (!ctx.started || ctx.performed) ^ ctx.canceled; //  y a false si deja de estarlo

        // When the key is pressed down.
        if (ctx.started && !ctx.performed)
        {
            spriteImage.enabled = true;
        }

        // When the key is lifted up.
        if (ctx.canceled && !ctx.performed) // Si se levanta la tecla antes de tiempo
        {
            spriteImage.fillAmount = 0;
            spriteImage.enabled = false;
        }
        return buttonPressed;
    }

    private bool CanBoostCurrentTower()
    {
        Ray rayo = Camera.main.ScreenPointToRay(GameUIManager.Instance.crossHead.transform.position);

        bool collidingWithTower = Physics.Raycast(rayo, out RaycastHit golpeRayo, PlaceManager.Instance.maxPlaceDistance,
            1 << GameManager.Instance.layerTorres);
        if (collidingWithTower) // Detectando la torre con la que se está chocando al presionar la tecla
        {
            IBoosteable torreBoosteable = golpeRayo.collider.gameObject.GetComponent<IBoosteable>();

            if (torreBoosteable != null)
            {
                int currentLvl = torreBoosteable.CurrentBoostLevel();
                int MaxLvl = torreBoosteable.MaxBoostLevel();
                if (currentLvl <= MaxLvl && torreBoosteable.HaEnoughMoneyForNextBoost())
                { // Si no se ha alcanzado el nivel máximo y tiene suficiente dinero para boostear, retorna true
                    return true;
                }
            }
        }
        return false; // Si no retorna false
    }

    private void SellWhenTimeCompleted()
    { InteractUntilTimeCompleted(false, spriteTowerSelling, new Action<RaycastHit>(Sell), sellingTowerTime); }

    private void BoostWhenTimeCompleted()
    {
        InteractUntilTimeCompleted(!canBoostCurrentTower, spriteTowerBoosting,
          new Action<RaycastHit>(Boost), sellingTowerTime);
    }

    private void InteractUntilTimeCompleted(bool ignore, Image fillingImage, Action<RaycastHit> method, float actionTime)
    {
        if (!ignore)
        {
            Ray rayo = Camera.main.ScreenPointToRay(GameUIManager.Instance.crossHead.transform.position);

            bool collidingWithTower = Physics.Raycast(rayo, out RaycastHit golpeRayo, PlaceManager.Instance.maxPlaceDistance,
                1 << GameManager.Instance.layerTorres);

            if (collidingWithTower && golpeRayo.collider.CompareTag("Tower")) // Si el raycast choca con una torre
            {
                fillingImage.fillAmount += Time.deltaTime / sellingTowerTime; // Añadir tiempo
                if (fillingImage.fillAmount >= 1) // Si llega al final del tiempo, se vende la torre añadiendo dinero
                {
                    method.Invoke(golpeRayo); // Se vende o boostea, dependiendo del tipo de Action que haya recibido
                }                                   // la función
            }
            else // Si el raycast deja de chocar con una torre, restaurar el contador a 0
            {
                fillingImage.fillAmount = 0;
            }
        }
    }

    private void Sell(RaycastHit golpeRayo)
    {
        Tower torre = golpeRayo.collider.gameObject.GetComponent<Tower>();
        IDamageable torreBoosteable = torre.GetComponent<IDamageable>();
        float proporcionDineroVida = 1;
        if (torreBoosteable != null)
        {
            float vidaActual = torreBoosteable.GetHealth();
            float vidaOriginal = torreBoosteable.GetMaxHealth();
            proporcionDineroVida = vidaActual / vidaOriginal;
        }
        float divisorPrecio = sellingPercentageAmount / 100;
        MoneyManager.Instance.AddMoney(Mathf.RoundToInt((torre.Money * divisorPrecio) * proporcionDineroVida));
        torre.ReturnToPool();
        spriteTowerSelling.fillAmount = 0;
    }

    private void Boost(RaycastHit golpeRayo)
    {
        Tower torre = golpeRayo.collider.gameObject.GetComponent<Tower>();
        IBoosteable torreBoosteable = torre.GetComponent<IBoosteable>();
        if (torreBoosteable != null && torreBoosteable.HaEnoughMoneyForNextBoost())
        {
            MoneyManager.Instance.RemoveMoney(Mathf.RoundToInt(torreBoosteable.NextBoostMoney()));
            torreBoosteable.Boost();
            canBoostCurrentTower = false;
        }

        spriteTowerBoosting.fillAmount = 0;
    }

    private void Update()
    {
        if (sellingButtonPressed) // Si se presiona el boton de vender, empieza a comprobar si el raycast está chocando con una torre
        {
            SellWhenTimeCompleted();
        }
        if (boostingButtonPressed)
        {
            BoostWhenTimeCompleted();
        }
    }

    private void Start()
    {
        spriteTowerSelling.fillAmount = 0;
        spriteTowerSelling.enabled = false;
    }
}
