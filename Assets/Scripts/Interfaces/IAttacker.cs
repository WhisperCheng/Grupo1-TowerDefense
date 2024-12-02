using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttacker
{
    // Esta es una interfaz orientada a poner un orden para ciertos m�todos al atacar y recibir da�o
    public void OnAttack();
    /// Aqu� va c�digo relacionado con efectos de part�culas, cambiar
    /// animaciones, etc, pero NO c�digo relacionado con hacer da�o a entidades
    /// o para instanciarlas.
    /// Para eso ya est� IDamageable.TakeDamage()

    //public void OnGetDamaged(); ??
}
