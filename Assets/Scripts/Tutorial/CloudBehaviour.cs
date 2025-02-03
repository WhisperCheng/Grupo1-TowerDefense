using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CloudBehaviour : MonoBehaviour
{
    public Vector3 targetPosition;  //Nueva posicion a la que se moverán las nubes
    public float moveTime = 2f;     //Tiempo que tarda en moverse
    public UnityEvent onCloudsMoved;

    //Metodo que activa el movimiento de las nubes
    public void MoveClouds()
    {
        LeanTween.move(gameObject, targetPosition, moveTime).setEase(LeanTweenType.easeOutSine).setOnComplete(() => onCloudsMoved.Invoke()); //Dispara un evento opcional al terminar
    }
}
