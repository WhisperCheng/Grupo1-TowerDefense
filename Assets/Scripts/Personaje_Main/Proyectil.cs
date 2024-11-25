using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public GameObject marcador; 
    public GameObject proyectilPrefab; 
    public Transform puntoDisparo; 
    public float velocidadProyectil = 20f;
    public PlaceManager placeManager;
    
    private void Start()
    {
        placeManager = FindAnyObjectByType<PlaceManager>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !placeManager.bloqueoDisparo) 
        {
            LanzarProyectil();
        }
    }

    void LanzarProyectil()
    {
        Ray rayo = Camera.main.ScreenPointToRay(marcador.transform.position);
        RaycastHit golpeRayo;

        Vector3 destino;
        if (Physics.Raycast(rayo, out golpeRayo))
        {
            destino = golpeRayo.point;
        }
        else
        {
            destino = rayo.GetPoint(100f);
        }

        // Crea el proyectil 
        //GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
        GameObject proyectil = MagicProjectilePool.Instance.GetMagicProjectile(puntoDisparo.position, Quaternion.identity);
        Vector3 direccion = (destino - puntoDisparo.position).normalized;
        proyectil.GetComponent<Rigidbody>().velocity = direccion * velocidadProyectil;
    }
    
}
