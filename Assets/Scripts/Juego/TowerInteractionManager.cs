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

    public Image spriteTowerInteractions;

    private bool sellingButtonPressed;

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
        // To keep the state in a boolean. // Cambia sellingButtonPressed a true si está siendo presionado, y a false si deja de estarlo
        sellingButtonPressed = (!ctx.started || ctx.performed) ^ ctx.canceled;

        // When the key is pressed down.
        if (ctx.started && !ctx.performed)
        {
            spriteTowerInteractions.enabled = true;
            Debug.Log("A");
        }

        // When the key is lifted up.
        if (ctx.canceled && !ctx.performed) // Si se levanta la tecla antes de tiempo
        {
            spriteTowerInteractions.fillAmount = 0;
            Debug.Log("B");
            spriteTowerInteractions.enabled = false;
        }
        // https://stackoverflow.com/questions/75216584/detect-when-key-lifted-new-unity-input-system
    }

    private void SellWhenTimeCompleted()
    {
        Ray rayo = Camera.main.ScreenPointToRay(GameUIManager.Instance.crossHead.transform.position);
        
        bool collidingWithTower = Physics.Raycast(rayo, out RaycastHit golpeRayo, PlaceManager.Instance.maxPlaceDistance,
            1 << GameManager.Instance.layerTorres);
        if (collidingWithTower && golpeRayo.collider.CompareTag("Tower")) // Si el raycast choca con una torre
        {
            
            spriteTowerInteractions.fillAmount += Time.deltaTime / sellingTowerTime; // Añadir tiempo
            if (spriteTowerInteractions.fillAmount >= 1) // Si llega al final del tiempo, se vende la torre añadiendo dinero
            {
                Sell(golpeRayo);
            }
        }
        else // Si el raycast deja de chocar con una torre, restaurar el contador a 0
        {
            spriteTowerInteractions.fillAmount = 0;
        }
    }

    private void Sell(RaycastHit golpeRayo)
    {
        Tower torre = golpeRayo.collider.gameObject.GetComponent<Tower>();

        IDamageable torreDamageable = torre.GetComponent<IDamageable>();
        float proporcionDineroVida = 1;
        if (torreDamageable != null)
        {
            float vidaActual = torreDamageable.GetHealth();
            float vidaOriginal = torreDamageable.GetMaxHealth();
            proporcionDineroVida = vidaActual / vidaOriginal;
        }
        float divisorPrecio = sellingPercentageAmount / 100;
        MoneyManager.Instance.AddMoney(Mathf.RoundToInt((torre.Money * divisorPrecio) * proporcionDineroVida));
        torre.ReturnToPool();
        spriteTowerInteractions.fillAmount = 0;
    }

    private void Update()
    {
        if (sellingButtonPressed) // Si se presiona el boton de vender, empieza a comprobar si el raycast está chocando con una torre
        { 
            SellWhenTimeCompleted();
        }
    }

    private void Start()
    {
        spriteTowerInteractions.fillAmount = 0;
        spriteTowerInteractions.enabled = false;
    }

}
