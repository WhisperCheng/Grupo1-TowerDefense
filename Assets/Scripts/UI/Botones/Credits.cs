using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Credits : MonoBehaviour
{
    Vector3 startPos;
    Vector3 globalStartPos;
    int finalPosY = 100;
    int posRect = 1000;
    [SerializeField] int speed = 12;
    // Start is called before the first frame update

    void Start()
    {
        startPos = transform.localPosition;
        globalStartPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * speed);
        if (transform.position.y > finalPosY)
        {
            float padreX = transform.position.x;
            float nuevaPosX = padreX + startPos.x;
            transform.position = new Vector3(nuevaPosX,startPos.y + posRect, globalStartPos.z);
        }
    }
}
