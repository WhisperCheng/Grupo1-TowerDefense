using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnAroundJavi : MonoBehaviour
{
    public Transform target; // Objeto alrededor del cual la c�mara rotar�
    public float rotationSpeed = 10f; // Velocidad de rotaci�n

    void Update()
    {
        if (target != null)
        {
            // Rotar la c�mara alrededor del objeto en el eje Y
            transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);

            // Mantener la c�mara mirando hacia el objeto
            transform.LookAt(target);
        }
    }
}

