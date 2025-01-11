using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackBox
{
    public Vector3 attackingBoxSize;
    public Vector3 attackingBoxPos;


    public bool AttackModeBool { get; private set; }
    public bool CanAttackOrDamageBool { get; private set; }
    public AttackBox() { }

    /// <summary>
    /// Ataca a los rivales cercanos manejando el animator, el daño y varios parámetros ingresados
    /// </summary>
    /// <param name="originGameObject"></param>
    /// <param name="masks"></param>
    /// <param name="animator"></param>
    /// <param name="canAttackOrDamageBool"></param>
    /// <param name="damage"></param>
    /// <param name="cooldown"></param>
    /// <returns>True si se ha realizado la acción de atacar</returns>
    public bool ManageAttack(GameObject originGameObject, int masks, Animator animator, bool canAttackOrDamageBool, float damage)
    {
        bool result = false;
        Transform transform = originGameObject.transform;
        // Centro relativo
        Vector3 center = transform.position + (transform.forward * attackingBoxPos.z) + (transform.right * attackingBoxPos.x) + (transform.up * attackingBoxPos.y);
        Collider[] rivals = Physics.OverlapBox(center, attackingBoxSize / 2, transform.rotation, masks); // OverlapBox respecto al centro relativo y la rotación

        if (rivals.Length > 0)
        {
            foreach (Collider col in rivals)
            {
                IDamageable damageableEntity = col.gameObject.GetComponent<IDamageable>();
                if (damageableEntity != null && damageableEntity.GetHealth() > 0) // Solo atacar si existe y tiene vida +> 0
                {
                    AttackModeBool = true;
                    animator.SetTrigger("Attack");
                    CanAttackOrDamageBool = canAttackOrDamageBool;
                    if (canAttackOrDamageBool)
                    {
                        damageableEntity.TakeDamage(damage);
                        CanAttackOrDamageBool = false;
                        AttackModeBool = false; // Reset del bool para hacer daño y del de modo de ataque
                        //CurrentCooldown = cooldown; // Reset del cooldown
                        result = true;
                    }
                }
            }
        }
        else
        {
            AttackModeBool = false;
            animator.ResetTrigger("Attack");
            result = false;
        }
        return result;
    }
}
