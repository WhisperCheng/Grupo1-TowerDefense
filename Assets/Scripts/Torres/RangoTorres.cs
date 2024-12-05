using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangoTorres : MonoBehaviour
{
    public Transform marcador; 
    public GameObject rangeIndicator;
    private float rangoTorre;
    private bool torreApuntada = false;

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
            GameObject torreCopiada = PlaceManager.Instance.torreCopiada;
            if (torreCopiada != null && torreCopiada.activeSelf)
            {
                Tower torreScript = PlaceManager.Instance.torreCopiada.GetComponent<Tower>();

                if (torreScript != null)
                {
                    rangoTorre = 2 * torreScript.range; 
                    rangeIndicator.transform.position = torreCopiada.transform.position;
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
                    rangoTorre = 2 * torre.GetComponent<Tower>().range;
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
