using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorUtils
{
    /*public static void ChangeObjectMaterialColors(GameObject gameObject, MaterialPropertyBlock propertyBlock)
    {
        Transform[] childs = gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            Transform child = childs[i];
            Renderer rend = child.gameObject.GetComponent<Renderer>();
            if (rend != null)
            {
                if (previewMode) // Si se está en modo selección, se cambia el PropertyBlock de materiales al de materiales seleccionados
                {
                    rend.SetPropertyBlock(materialesSeleccionados);
                }
                else // Y si deja de estarlo, se quita el PropertyBlock de materiales seleccionados
                {
                    rend.SetPropertyBlock(null);
                }
            }
        }
    }*/
}
