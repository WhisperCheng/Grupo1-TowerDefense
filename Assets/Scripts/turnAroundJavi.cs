using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnAroundJavi : MonoBehaviour
{
    public Transform target; // Objeto alrededor del cual la cámara rotará
    public float rotationSpeed = 10f; // Velocidad de rotación

    void Update()
    {
        if (target != null)
        {
            // Rotar la cámara alrededor del objeto en el eje Y
            transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);

            // Mantener la cámara mirando hacia el objeto
            transform.LookAt(target);
        }
    }
}

