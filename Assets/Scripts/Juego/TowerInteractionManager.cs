using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TowerInteractionManager : MonoBehaviour
{
    [Header("Porcentaje de gemas recibidas al vender torre")]
    [Range(0, 100)]
    public float sellingPercentageAmount;

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
        if (ctx.performed)
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
        }

        //if (context.canceled && !context.performed) // Si se levanta la tecla antes de tiempo
    }
}
