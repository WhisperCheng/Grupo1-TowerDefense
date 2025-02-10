using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RangoTorres : MonoBehaviour
{
    //UI
    public GameObject ventaPanel;
    //IN GAME
    public Transform marcador; 
    public GameObject rangeIndicator;
    private float rangoTorre;
    private bool torreApuntada = false;
    public TextMeshProUGUI boostPriceText;
    private float boostPrice;
    public TextMeshProUGUI sellPriceText;
    private float sellPrice;

    private void Start()
    {
        rangeIndicator = Instantiate(rangeIndicator);
    }

    void Update()
    {
        torreApuntada = false;
        bool objetoSiendoArrastrado = PlaceManager.Instance.objetoSiendoArrastrado;
        if (objetoSiendoArrastrado)
        {
            GameObject gameObjTorreCopiada = PlaceManager.Instance.GetCurrentManagedTower().gameObject;
            if (gameObjTorreCopiada != null && gameObjTorreCopiada.activeSelf)
            {
                Tower torre = PlaceManager.Instance.GetCurrentManagedTower();

                if (torre != null)
                {
                    rangoTorre = 2 * torre.GetRange(); 
                    rangeIndicator.transform.position = gameObjTorreCopiada.transform.position;
                    rangeIndicator.transform.localScale = new Vector3(rangoTorre, rangoTorre, rangoTorre);
                    rangeIndicator.gameObject.SetActive(true);
                    torreApuntada = true;
                }
            }
        }
        else
        {
            int towerMask = 1 << GameManager.Instance.layerTorres;
            int trapMask = 1 << GameManager.Instance.layerTrap;
            Ray rayo = Camera.main.ScreenPointToRay(marcador.position);
            if (Physics.Raycast(rayo, out RaycastHit golpeRayo, PlaceManager.Instance.maxPlaceDistance, towerMask | trapMask))
            {
                if (golpeRayo.collider.CompareTag(GameManager.Instance.tagTorres) 
                    || golpeRayo.collider.CompareTag(GameManager.Instance.tagTorresCamino))
                {
                    rangeIndicator.gameObject.SetActive(true);
                    GameObject gameObjTorre = golpeRayo.collider.gameObject;
                    Tower tower = gameObjTorre.GetComponent<Tower>();
                    boostPrice = tower is LivingTower ? ((LivingTower)tower).NextBoostMoney() : 0; // Boost
                    boostPriceText.text = tower ? (boostPrice <= 0 ? "-" : boostPrice.ToString()) : "-";

                    IDamageable torreDamageable = tower.GetComponent<IDamageable>();
                    float proporcionDineroVida = 1;
                    if (torreDamageable != null)
                    {
                        float vidaActual = torreDamageable.GetHealth();
                        float vidaOriginal = torreDamageable.GetMaxHealth();
                        proporcionDineroVida = vidaActual / vidaOriginal;
                    }
                    float divisorPrecio = TowerInteractionManager.Instance.sellingPercentageAmount / 100; // Venta
                    sellPrice = Mathf.RoundToInt((tower.Money * divisorPrecio)* proporcionDineroVida);
                    sellPriceText.text = sellPrice.ToString();
                    rangoTorre = 2 * tower.GetRange();
                    rangeIndicator.transform.position = gameObjTorre.transform.position;
                    rangeIndicator.transform.localScale = new Vector3(rangoTorre, rangoTorre, rangoTorre);
                    ventaPanel.gameObject.SetActive(true);
                    torreApuntada = true;
                }
            }
        }

        if (!torreApuntada)
        {
            rangeIndicator.gameObject.SetActive(false);
            ventaPanel.gameObject.SetActive(false);
        }
    }
}
