using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttacker
{
    // Esta es una interfaz orientada a poner un orden para ciertos métodos al atacar y recibir daño
    public void OnAttack();
    /// Aquí va código relacionado con efectos de partículas, cambiar
    /// animaciones, etc, pero NO código relacionado con hacer daño a entidades
    /// o para instanciarlas.
    /// Para eso ya está IDamageable.TakeDamage()

    //public void OnGetDamaged(); ??
}
