using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangoTorres : MonoBehaviour
{
    public Transform marcador; 
    public GameObject rangeIndicator;
    private float rangoTorre;
    private bool torreApuntada = false;
    public PlaceManager placeManager; 

    private void Start()
    {
        rangeIndicator = Instantiate(rangeIndicator);
        placeManager = FindAnyObjectByType<PlaceManager>();
    }

    void Update()
    {
        torreApuntada = false;

        if (placeManager.objetoSiendoArrastrado)
        {
            if (placeManager.objetoCopiado != null && placeManager.objetoCopiado.activeSelf)
            {
                Tower torreScript = placeManager.objetoCopiado.GetComponent<Tower>();

                if (torreScript != null)
                {
                    rangoTorre = 2 * torreScript.rango; 
                    rangeIndicator.transform.position = placeManager.objetoCopiado.transform.position;
                    rangeIndicator.transform.localScale = new Vector3(rangoTorre, rangoTorre, rangoTorre);
                    rangeIndicator.gameObject.SetActive(true); 
                    torreApuntada = true;
                }
            }
        }
        else
        {
            Ray rayo = Camera.main.ScreenPointToRay(marcador.position);
            if (Physics.Raycast(rayo, out RaycastHit golpeRayo))
            {
                if (golpeRayo.collider.CompareTag("Tower"))
                {
                    rangeIndicator.gameObject.SetActive(true);
                    GameObject torre = golpeRayo.collider.gameObject;
                    rangoTorre = 2 * torre.GetComponent<Tower>().rango; 
                    rangeIndicator.transform.position = torre.transform.position;
                    rangeIndicator.transform.localScale = new Vector3(rangoTorre, rangoTorre, rangoTorre);
                    torreApuntada = true;
                }
            }
        }

        if (!torreApuntada)
        {
            rangeIndicator.gameObject.SetActive(false);
        }
    }
}
