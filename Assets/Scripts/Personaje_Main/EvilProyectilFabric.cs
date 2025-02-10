using FMODUnity;
using UnityEngine;

public class EvilProyectilFabric : MonoBehaviour
{
    public Transform puntoDisparo;
    public float velocidadProyectilDefault = 30f;
    private void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void LanzarEvilMagicProyectil(Vector3 destino, float velocidadProyectil) // Usado por el MagicArcherAI
    {
        PlayProyectileShootSound();

        // Crea el proyectil 
        GameObject proyectil = GetProyectileFromPool();
        proyectil.transform.position = puntoDisparo.position;
        Vector3 direccion = (destino - puntoDisparo.position).normalized;
        proyectil.GetComponent<Rigidbody>().velocity = direccion * velocidadProyectil;
    }

    public void LanzarEvilMagicProyectil(Vector3 destino)
    {
        LanzarEvilMagicProyectil(destino, velocidadProyectilDefault);
    }

    protected GameObject GetProyectileFromPool()
    {
        return EvilMagicProjectilePool.Instance.GetMagicProjectile(puntoDisparo.position, Quaternion.identity);
    }

    protected void PlayProyectileShootSound()
    {
        //FMOD
        AudioManager.instance.PlayOneShot(FMODEvents.instance.magicAttack, this.transform.position);
    }
}
