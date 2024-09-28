using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class MovPersonaje2D : MonoBehaviour
{
    //public float speed = 5.0f; // La velocidad a la que se va a mover el personaje
    //private float speed; // La velocidad a la que se va a mover el personaje
    public float turnSpeed = 300.0f; // La velocidad de giro
    //public float originalSpeed = 5.0f;

    private CharacterController characterController;
    private Animator animatorController;
    //bool controlHabilitado;

    Vector3 movCharacter;
    private float velocidadGravedad = 1f;
    private float gravedadMagnitud = -9.81f;
    private float gravedadMultiplicador = 0.75f;

    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    public float acceleration = 2.0f;
    //public float deceleration = 2.0f;
    public float maximumWalkVelocity = 4f;
    public float maximumRunVelocity = 10f;

    int VelocityZHash;
    int VelocityXHash;

    void Start()
    {
        // Get the Character Controller on the player
        characterController = GetComponent<CharacterController>();
        animatorController = GetComponent<Animator>();
        //speed = maximumWalkVelocity;
        //controlHabilitado = false;
        VelocityZHash = Animator.StringToHash("VelZ");
        VelocityXHash = Animator.StringToHash("VelX");
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float horizontalCamera = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Vertical");

        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool leftPressed = Input.GetKeyDown(KeyCode.A);
        bool backwardsPressed = Input.GetKeyDown(KeyCode.S);
        
        //TODO: https://www.youtube.com/watch?v=_J8RPIaO2Lc
        
        // Rotamos en el eje Y
        transform.Rotate(0, horizontalCamera * turnSpeed * Time.deltaTime, 0);

        // Calculamos el vector de movimiento -> ((0,0,1) * vertical + (1,0,0) * horizontal) * speed
        movCharacter = transform.forward * vertical + transform.right * horizontal;
        applyGravity();

        // Controlando si intenta correr
        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;
        // Mover personaje
        characterController.Move(movCharacter * currentMaxVelocity * Time.deltaTime);

        //float magnitudMov = Mathf.Clamp(characterController.velocity.magnitude, 0, maximumRunVelocity);

        //animatorController.SetFloat("VelX", magnitudMov * (Input.GetKey(KeyCode.D) ? -1 : 1));
        //animatorController.SetFloat("VelZ", magnitudMov * (Input.GetKey(KeyCode.S) ? -1 : 1));
        velocityX = Mathf.Lerp(velocityX,
            horizontal * currentMaxVelocity, Time.deltaTime * acceleration);
        velocityZ = Mathf.Lerp(velocityZ,
            vertical * currentMaxVelocity, Time.deltaTime * acceleration);

        if (velocityX < 0.001f && velocityX > -0.001f)
        {
            velocityX = 0f;
        }

        if (velocityZ < 0.001f && velocityZ > -0.001f)
        {
            velocityZ = 0f;
        }

        animatorController.SetFloat(VelocityZHash, velocityZ);
        animatorController.SetFloat(VelocityXHash, velocityX);
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
