using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        int pathMask = 1 << GameManager.Instance.layerPath;
        int terrainMask = 1 << GameManager.Instance.layerTerreno;
        int areaDecoMask = 1 << GameManager.Instance.layerAreaDeco;
        if (Physics.Raycast(rayo, out golpeRayo, Mathf.Infinity, pathMask | terrainMask | areaDecoMask) 
            && !golpeRayo.transform.CompareTag(GameManager.Instance.tagPuentes))
        {
            destino = golpeRayo.point;
        }
        else
        {
            // Pos + bordes para apuntar exactamente al centro de la mira
            rayo = Camera.main.ScreenPointToRay(marcador.transform.position + marcador.GetComponent<Image>().sprite.bounds.extents);
            destino = rayo.GetPoint(10000);
        }

        // Crea el proyectil 
        //GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
        GameObject proyectil = MagicProjectilePool.Instance.GetMagicProjectile(puntoDisparo.position, Quaternion.identity);
        proyectil.transform.position = puntoDisparo.position;
        Vector3 direccion = (destino - puntoDisparo.position).normalized;
        proyectil.GetComponent<Rigidbody>().velocity = direccion * velocidadProyectil;
    }
    
}
