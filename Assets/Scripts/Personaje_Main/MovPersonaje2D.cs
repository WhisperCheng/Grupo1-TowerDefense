using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class MovPersonaje2D : MonoBehaviour
{
    //public float speed = 5.0f; // La velocidad a la que se va a mover el personaje
    private float speed; // La velocidad a la que se va a mover el personaje
    public float turnSpeed = 300.0f; // La velocidad de giro
    public float originalSpeed = 5.0f;


    private CharacterController characterController;
    private Animator animatorController;
    bool controlHabilitado;

    Vector3 movCharacter;
    private float velocidadGravedad = 1f;
    private float gravedadMagnitud = -9.81f;
    private float gravedadMultiplicador = 0.75f;

    void Start()
    {
        // Get the Character Controller on the player
        characterController = GetComponent<CharacterController>();
        animatorController = GetComponent<Animator>();
        speed = originalSpeed;
        controlHabilitado = false;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float horizontalCamera = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Vertical");

        // Controlando si intenta correr
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = originalSpeed + 20f;
        }
        else
        {
            speed = originalSpeed;

        }

        //TODO: https://www.youtube.com/watch?v=_J8RPIaO2Lc
        /*
          Reemplazar ChangeVelocity() y LockOrResetVelocity() por : 
            velocityX = Mathf.Lerp(velocityX, input.x * currentMaxVelocity, Time.deltaTime * acceleration);
            velocityZ = Mathf.Lerp(velocityZ, input.y * currentMaxVelocity, Time.deltaTime * acceleration);
         */

        // Rotamos en el eje Y
        transform.Rotate(0, horizontalCamera * turnSpeed * Time.deltaTime, 0);

        // Calculamos el vector de movimiento -> ((0,0,1) * vertical + (1,0,0) * horizontal) * speed
        movCharacter = transform.forward * vertical + transform.right * horizontal;
        applyGravity();

        characterController.Move(movCharacter * speed * Time.deltaTime);

        animatorController.SetFloat("VelX", characterController.velocity.magnitude *
            (Input.GetKey(KeyCode.D) ? -1 : 1));

        //Debug.Log(characterController.velocity.magnitude *(Input.GetKey(KeyCode.S) ? -1 : 1));
        animatorController.SetFloat("VelZ", characterController.velocity.magnitude *
            (Input.GetKey(KeyCode.S) ? -1 : 1));
    }

    private void applyGravity()
    {
        if (characterController.isGrounded)
        {
            velocidadGravedad = -1f;
        }
        else
        {
            velocidadGravedad += gravedadMagnitud * gravedadMultiplicador * Time.deltaTime;
        }
        movCharacter.y = velocidadGravedad;
        //characterController.Move(new Vector3(0f, velocidadGravedad, 0f));
    }
}
