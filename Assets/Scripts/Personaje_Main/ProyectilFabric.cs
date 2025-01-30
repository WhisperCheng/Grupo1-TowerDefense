using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ProyectilFabric : MonoBehaviour
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
        
    }

    public void LanzarProyectil()
    {
        Ray rayo = Camera.main.ScreenPointToRay(marcador.transform.position);
        RaycastHit golpeRayo;

        //FMOD
        AudioManager.instance.PlayOneShot(FMODEvents.instance.magicAttack, this.transform.position);

        Vector3 destino;
        int pathMask = 1 << GameManager.Instance.layerPath;
        int terrainMask = 1 << GameManager.Instance.layerTerreno;
        int areaDecoMask = 1 << GameManager.Instance.layerAreaDeco;
        int enemyMask = 1 << GameManager.Instance.layerEnemigos;
        int towerMask = 1 << GameManager.Instance.layerTorres;
        int hearthMask = 1 << GameManager.Instance.layerCorazon;
        if (Physics.Raycast(rayo, out golpeRayo, Mathf.Infinity, pathMask | terrainMask | areaDecoMask | enemyMask
            | towerMask | hearthMask))
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
