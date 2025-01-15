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
        /*if (ctx.performed)
        {
            Ray rayo = Camera.main.ScreenPointToRay(GameUIManager.Instance.crossHead.transform.position);
            if (Physics.Raycast(rayo, out RaycastHit golpeRayo))
            {
                if (golpeRayo.collider.CompareTag("Tower"))
                {
                    Tower torre = golpeRayo.collider.gameObject.GetComponent<Tower>();
                    
                    IDamageable torreDamageable = torre.GetComponent<IDamageable>();
                    float proporcionDineroVida = 1;
                    if (torreDamageable != null)
                    {
                        float vidaActual = torreDamageable.GetHealth();
                        float vidaOriginal = torreDamageable.GetHealth();
                        proporcionDineroVida = vidaActual / vidaOriginal;
                    }
                    float divisorPrecio = sellingPercentageAmount / 100;
                    MoneyManager.Instance.AddMoney(Mathf.RoundToInt((torre.Money * divisorPrecio) * proporcionDineroVida));
                    torre.ReturnToPool();
                }
            }
        }*/

        // To keep the state in a boolean.
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

    private void Update()
    {
        if (sellingButtonPressed)
        {
            spriteTowerInteractions.fillAmount += Time.deltaTime;
        }
        /*else
        {
            spriteTowerInteractions.fillAmount = 0;
        }*/
    }

}
