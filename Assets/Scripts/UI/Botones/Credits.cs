using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Credits : MonoBehaviour
{
    Vector3 startPos;
    int finalPos = 814;
    [SerializeField] int speed = 700;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        //Lucía María Alemán Marrero
        //Carlota Carballo Falcón
        //Omar Chávez González
        //Sergio Delgado Salas
        //Iván Domínguez Díaz
        //Ángel Jesús Garnica Paz
        //David Alejandro Hernández Alonso
        //Idayra Inmaculada Pérez Delgado
        //Conservatorio S/C de Tenerife
        //Concept Artist + Game Designer + Main programmer + GUI Designer + Sound Designer + Sound Engineer + Particle Designer + GUI Integrators
        //+ Modeling artists + Programmers + Animator 
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime * speed);
        if (transform.position.y > finalPos)
        {
            transform.position = startPos;
        }
    }
}
