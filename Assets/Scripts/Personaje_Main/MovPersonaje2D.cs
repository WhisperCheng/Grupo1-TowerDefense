using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine.InputSystem;
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

    PlayerInput playerInput;
    private Vector2 moveDirection;
    private Vector2 smoothMoveDirection;
    private Vector2 lookDirection;
    private Vector2 smoothedMoveSpeed;
    public float smoothingMoveSpeed = 0.2f;

    void Start()
    {
        // Get the Character Controller on the player
        characterController = GetComponent<CharacterController>();
        animatorController = GetComponent<Animator>();
        //speed = maximumWalkVelocity;
        //controlHabilitado = false;
        VelocityZHash = Animator.StringToHash("VelZ");
        VelocityXHash = Animator.StringToHash("VelX");

        playerInput = GetComponent<PlayerInput>();
    }
    void FixedUpdate()
    {
        /*float horizontal = Input.GetAxis("Horizontal");
        float horizontalCamera = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Vertical");*/
    }


    void Update()
    {
        moveDirection = playerInput.actions["Move"].ReadValue<Vector2>();
        lookDirection = playerInput.actions["Look"].ReadValue<Vector2>();

        //bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool runPressed = playerInput.actions["Correr"].ReadValue<float>() > 0 ? true : false;
        bool leftPressed = Input.GetKeyDown(KeyCode.A);
        bool backwardsPressed = Input.GetKeyDown(KeyCode.S);
        
        //TODO: https://www.youtube.com/watch?v=_J8RPIaO2Lc
        
        // Rotamos en el eje Y
        transform.Rotate(0, lookDirection.x * turnSpeed * Time.deltaTime, 0);

        //Suavizar el movimiento del vector moveDirection
        smoothMoveDirection = Vector2.SmoothDamp(smoothMoveDirection, moveDirection, 
            ref smoothedMoveSpeed, smoothingMoveSpeed);

        // Calculamos el vector de movimiento -> ((0,0,1) * vertical + (1,0,0) * horizontal) * speed
        movCharacter = transform.forward * smoothMoveDirection.y + transform.right * 
            smoothMoveDirection.x;
        applyGravity();

        // Controlando si intenta correr
        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;
        // Mover personaje
        characterController.Move(movCharacter * currentMaxVelocity * Time.deltaTime);

        // Suavizar transiciones entre animaciones
        velocityX = Mathf.Lerp(velocityX,
            moveDirection.x * currentMaxVelocity, Time.deltaTime * acceleration);
        velocityZ = Mathf.Lerp(velocityZ,
            moveDirection.y * currentMaxVelocity, Time.deltaTime * acceleration);

        if (velocityX < 0.001f && velocityX > -0.001f)
        {
            velocityX = 0f;
        }

        if (velocityZ < 0.001f && velocityZ > -0.001f)
        {
            velocityZ = 0f;
        }

        animatorController.SetFloat(VelocityZHash, velocityX);
        animatorController.SetFloat(VelocityXHash, velocityZ);
    }

    private void applyGravity()
    {
        if (characterController.isGrounded)
        {
            velocidadGravedad = 0f;
        }
        else
        {
            velocidadGravedad += gravedadMagnitud * gravedadMultiplicador * Time.deltaTime;
        }
        movCharacter.y = velocidadGravedad;
        //characterController.Move(new Vector3(0f, velocidadGravedad, 0f));
    }
}
