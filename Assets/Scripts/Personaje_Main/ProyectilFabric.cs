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
        PlayProyectileShootSound();

        Vector3 destino = GetClosestImpactPoint();

        // Crea el proyectil 
        //GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
        GameObject proyectil = GetProyectileFromPool();
        proyectil.transform.position = puntoDisparo.position;
        Vector3 direccion = (destino - puntoDisparo.position).normalized;
        proyectil.GetComponent<Rigidbody>().velocity = direccion * velocidadProyectil;
    }

    protected virtual GameObject GetProyectileFromPool()
    {
        return MagicProjectilePool.Instance.GetMagicProjectile(puntoDisparo.position, Quaternion.identity);
    }

    protected virtual void PlayProyectileShootSound()
    {
        //FMOD
        AudioManager.instance.PlayOneShot(FMODEvents.instance.magicAttack, this.transform.position);
    }

    private Vector3 GetClosestImpactPoint()
    {
        Vector3 destino;

        Ray rayo = Camera.main.ScreenPointToRay(marcador.transform.position);

        int pathMask = 1 << GameManager.Instance.layerPath;
        int terrainMask = 1 << GameManager.Instance.layerTerreno;
        int areaDecoMask = 1 << GameManager.Instance.layerAreaDeco;
        int enemyMask = 1 << GameManager.Instance.layerEnemigos;
        int towerMask = 1 << GameManager.Instance.layerTorres;
        int hearthMask = 1 << GameManager.Instance.layerCorazon;

        RaycastHit[] hits = Physics.RaycastAll(rayo, Mathf.Infinity, pathMask | terrainMask | areaDecoMask | enemyMask
            | towerMask | hearthMask);

        RaycastHit currentHit;
        currentHit = new RaycastHit();
        currentHit.distance = Mathf.Infinity;
        foreach (RaycastHit hit in hits) // Se recorren todas las colisiones y se obtiene la más cercana que no tenga
        {                                                                                // el tag de cajas de puente
            if (hit.distance < currentHit.distance && !hit.transform.CompareTag(GameManager.Instance.tagPuentes))
            {
                currentHit = hit;
            }
        }
        if (currentHit.distance != Mathf.Infinity) // Si se ha encontrado algo con lo que apuntar
        {
            destino = currentHit.point;
        }
        else // Si no se lanza hacia el infinito mirando hacia delante
        {
            // Pos + bordes para apuntar exactamente al centro de la mira
            rayo = Camera.main.ScreenPointToRay(marcador.transform.position + marcador.GetComponent<Image>().sprite.bounds.extents);
            destino = rayo.GetPoint(10000);
        }
        return destino;
    }
}
