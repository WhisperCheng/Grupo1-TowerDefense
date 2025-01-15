using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // Interfaz destinada al instanciamiento y uso de los métodos a implementar para la gestión
    // del daño de cada entidad
    public void Die();
    public void TakeDamage(float damageAmount);
    public float GetHealth();
    public float GetMaxHealth();
    //public void RestoreHealth();
}
