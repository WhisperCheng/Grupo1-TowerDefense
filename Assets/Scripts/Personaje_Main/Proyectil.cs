using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public GameObject marcador; 
    public GameObject proyectilPrefab; 
    public Transform puntoDisparo; 
    public float velocidadProyectil = 20f;

    private void Start()
    {

    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !PlaceManager.Instance.bloqueoDisparo) 
        {
            LanzarProyectil();
        }
    }

    void LanzarProyectil()
    {
        Ray rayo = Camera.main.ScreenPointToRay(marcador.transform.position);
        RaycastHit golpeRayo;

        //FMOD
        AudioManager.instance.PlayOneShot(FMODEvents.instance.magicAttack, this.transform.position);

        Vector3 destino;
        if (Physics.Raycast(rayo, out golpeRayo, Mathf.Infinity, 1 << GameManager.Instance.layerPath))
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
        proyectil.transform.position = puntoDisparo.position;
        Vector3 direccion = (destino - puntoDisparo.position).normalized;
        proyectil.GetComponent<Rigidbody>().velocity = direccion * velocidadProyectil;
    }
    
}
