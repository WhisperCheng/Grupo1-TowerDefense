using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackBox
{
    public Vector3 attackingBoxSize;
    public Vector3 attackingBoxPos;


    public bool AttackModeBool { get; private set; }
    public GameObject AttackedEntity { get; private set; }
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
    public bool ManageAttack(Transform mainObject, Transform rotation, int masks, Animator animator, bool canAttackOrDamageBool, float damage)
    {
        if (rotation == null)
            rotation = mainObject;
        bool result = false;
        // Centro relativo
        Vector3 center = mainObject.position + (mainObject.forward * attackingBoxPos.z) + (mainObject.right * attackingBoxPos.x) + (mainObject.up * attackingBoxPos.y);
        Collider[] rivals = Physics.OverlapBox(center, attackingBoxSize / 2, rotation.rotation, masks); // OverlapBox respecto al centro relativo y la rotación

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
                        AttackedEntity = col.gameObject;
                        CanAttackOrDamageBool = false;
                        AttackModeBool = false; // Reset del bool para hacer daño y del de modo de ataque
                        //CurrentCooldown = cooldown; // Reset del cooldown
                        result = true;
                    }
                }
                else { animator.ResetTrigger("Attack"); }
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

    public bool ManageAttack(Transform mainObject, int masks, Animator animator, bool canAttackOrDamageBool, float damage)
    {
        return ManageAttack(mainObject, null, masks, animator, canAttackOrDamageBool, damage);
    }

    public void DrawGizmos(Transform mainObject)
    {
        Color prevColor = Gizmos.color;
        Matrix4x4 prevMatrix = Gizmos.matrix;

        Gizmos.color = Color.red;
        Vector3 matrixCenter = mainObject.position + (mainObject.forward * attackingBoxPos.z) + (mainObject.right * attackingBoxPos.x)
            + (mainObject.up * attackingBoxPos.y);
        Vector3 center = matrixCenter;

        Gizmos.matrix = mainObject.localToWorldMatrix;
        matrixCenter = mainObject.InverseTransformPoint(matrixCenter); // convert from world position to local position 
        Gizmos.DrawWireCube(matrixCenter, attackingBoxSize);

        Gizmos.matrix = prevMatrix;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(mainObject.position, center);
        Gizmos.color = prevColor;
    }
}
