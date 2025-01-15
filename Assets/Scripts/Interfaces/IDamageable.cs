using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // Interfaz destinada al instanciamiento y uso de los m�todos a implementar para la gesti�n
    // del da�o de cada entidad
    public void Die();
    public void TakeDamage(float damageAmount);
    public float GetHealth();
    public float GetMaxHealth();
    //public void RestoreHealth();
}
