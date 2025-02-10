using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorUtils
{
    public static void ChangeObjectMaterialColors(GameObject mainObject, MaterialPropertyBlock propertyBlock, bool condition)
    {
        Transform[] childs = mainObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            Transform child = childs[i];
            Renderer rend = child.gameObject.GetComponent<Renderer>();
            if (rend != null)
            {
                if (condition && propertyBlock != null) // Si se está en modo selección (condition=true), se cambia el 
                {                                           // PropertyBlock de materiales al de materiales seleccionados
                    rend.SetPropertyBlock(propertyBlock);
                }
                else // Y si deja de estarlo, se quita el PropertyBlock de materiales seleccionados
                {
                    rend.SetPropertyBlock(null);
                }
            }
        }
    }

    public static void ChangeObjectMaterialColors(GameObject mainObject, MaterialPropertyBlock propertyBlock)
    {
        ChangeObjectMaterialColors(mainObject, propertyBlock, true);
    }

    public static MaterialPropertyBlock CreateToonShaderPropertyBlock(Color color)
    {
        // -- Veneno --
        MaterialPropertyBlock materialProperty = new MaterialPropertyBlock();
        materialProperty.SetColor("_BaseColor", color);
        materialProperty.SetColor("_Color", color); // por si el material no tiene el toon shader
        // Sombras
        materialProperty.SetColor("_1st_ShadeColor", color);
        materialProperty.SetColor("_2nd_ShadeColor", color);
        return materialProperty;
    }

    public static void GetPoisonColor()
    {

    }
}
