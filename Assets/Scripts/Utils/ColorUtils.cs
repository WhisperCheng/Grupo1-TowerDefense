using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorUtils
{
    public static void ChangeObjectMaterialColors(GameObject mainObject, MaterialPropertyBlock propertyBlock)
    {
        Transform[] childs = mainObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            Transform child = childs[i];
            Renderer rend = child.gameObject.GetComponent<Renderer>();
            if (rend != null)
            {
                if (propertyBlock != null) // Si se está en modo selección, se cambia el PropertyBlock de materiales al de materiales seleccionados
                {
                    rend.SetPropertyBlock(propertyBlock);
                }
                else // Y si deja de estarlo, se quita el PropertyBlock de materiales seleccionados
                {
                    rend.SetPropertyBlock(null);
                }
            }
        }
    }

    public static void GetPoisonColor()
    {

    }
}
