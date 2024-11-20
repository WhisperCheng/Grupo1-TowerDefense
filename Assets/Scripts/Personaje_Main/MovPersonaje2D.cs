using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine.InputSystem;
using UnityEngine;

public class MovPersonaje2D : MonoBehaviour
{
    [Header("Giro")]
    [SerializeField] private float _turnSpeed = 40.0f; // La velocidad de giro
    [Range(0f, 60f)]
    [SerializeField] private float _smoothTurnSpeed = 4f;

    [Header("Animaciones")]
    [SerializeField] private CharacterController _characterController;
    private Animator _animatorController;

    [Header("Gravedad")]
    [SerializeField] private float _velocidadGravedad = 1f;
    [SerializeField] private float _gravedadMagnitud = -9.81f;
    [SerializeField] private float _gravedadMultiplicador = 1f;

    [Header("Movimiento")]
    [SerializeField] private float _acceleration = 2.0f;
    //public float deceleration = 2.0f;
    [SerializeField] private float _maximumWalkVelocity = 4f;
    [SerializeField] private float _maximumRunVelocity = 10f;

    [Header("Suavizado y movimiento en pendientes")]
    [SerializeField] private float _smoothingMoveSpeed = 0.2f;
    [SerializeField] private float _slopeForce;
    [SerializeField] private float _slopeForceRayLength;

    private Vector3 _movCharacter;
    private float _velocityZ = 0.0f;
    private float _velocityX = 0.0f;
    private int _velocityZHash;
    private int _velocityXHash;

    PlayerInput playerInput;
    private Vector2 _moveDirection;
    private Vector2 _smoothMoveDirection;
    private Vector2 _lookDirection;
    private float _oldLookDirection = 0f;
    private Vector2 _smoothedMoveSpeed;
    
    //float rotateYAngle = 0f;

    void Start()
    {
        // Get the Character Controller on the player
        _characterController = GetComponent<CharacterController>();
        _animatorController = GetComponent<Animator>();
        _velocityZHash = Animator.StringToHash("VelZ");
        _velocityXHash = Animator.StringToHash("VelX");

        playerInput = GetComponent<PlayerInput>();
    }
    void FixedUpdate()
    {

    }


    void Update()
    {
        _moveDirection = playerInput.actions["Move"].ReadValue<Vector2>();
        _lookDirection = playerInput.actions["Look"].ReadValue<Vector2>();

        /// Si el jugador no está tocando el suelo, se deshabilita temporalmente el control
        /// del movimiento horizontal
        /// // TODO
        /*if (!_characterController.isGrounded)
        {
            _moveDirection = Vector2.zero;
            _lookDirection = Vector2.zero;
        }*/

        //bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool runPressed = playerInput.actions["Correr"].ReadValue<float>() > 0 ? true : false;

        // Rotamos en el eje Y, se rota suavemente
        _lookDirection.x = Mathf.Lerp(_oldLookDirection, _lookDirection.x, (60.1f - _smoothTurnSpeed) * Time.deltaTime); // suavizar rotación
        float addRotation = _lookDirection.x * _turnSpeed * Time.deltaTime;
        transform.Rotate(0, addRotation, 0); // rotación con suavizado
        _oldLookDirection = _lookDirection.x;

        //Suavizar el movimiento del vector moveDirection
        _smoothMoveDirection = Vector2.SmoothDamp(_smoothMoveDirection, _moveDirection, 
            ref _smoothedMoveSpeed, _smoothingMoveSpeed);

        _movCharacter = transform.forward * _smoothMoveDirection.y + transform.right * 
            _smoothMoveDirection.x;
        applyGravity();

        // Controlando si intenta correr
        float currentMaxVelocity = runPressed ? _maximumRunVelocity : _maximumWalkVelocity;

        // Mover personaje
        Vector3 mov = new Vector3(_movCharacter.x * currentMaxVelocity, _movCharacter.y,
            _movCharacter.z * currentMaxVelocity);

        Vector3 slopeFix = Vector3.zero;
        /// Si el jugador se está moviendo y está en una rampa, se aplica un movimiento correctivo para
        /// que pueda caminar o deslizarse por ella correctamente si está dentro del ángulo de poder caminar por rampas
        if ((_movCharacter.x != 0 || _movCharacter.z != 0) && OnSlope())
        {
            slopeFix = Vector3.down * /*characterController.height / 2 * */ _slopeForce * Time.deltaTime;
        }
        
        _characterController.Move((mov + slopeFix) * Time.deltaTime);

        // Suavizar transiciones entre animaciones
        _velocityX = Mathf.Lerp(_velocityX,
            _moveDirection.x * currentMaxVelocity, Time.deltaTime * _acceleration);
        _velocityZ = Mathf.Lerp(_velocityZ,
            _moveDirection.y * currentMaxVelocity, Time.deltaTime * _acceleration);

        /// Si la velocidad es menor que 0.001 pasarla directamente a 0 para no estar
        ///  suavizando números más pequeños
        if (_velocityX < 0.001f && _velocityX > -0.001f)
        {
            _velocityX = 0f;
        }
        if (_velocityZ < 0.001f && _velocityZ > -0.001f)
        {
            _velocityZ = 0f;
        }

        _animatorController.SetFloat(_velocityZHash, _velocityX);
        _animatorController.SetFloat(_velocityXHash, _velocityZ);
    }

    // Aplicar gravedad al personaje
    private void applyGravity()
    {
        if (_characterController.isGrounded)
        {
            _velocidadGravedad = 0f;
        }
        else
        {
            _velocidadGravedad += _gravedadMagnitud * _gravedadMultiplicador * Time.deltaTime;
        }
        _movCharacter.y = _velocidadGravedad;
    }

    // Detectar si el jugador está en un terreno inclinado
    private bool OnSlope()
    {
        // TODO ?
        /*if (isJumping)
            return false;*/

        RaycastHit hit;

        /// Usa la posición del personaje partiendo desde su base para hacer un raycast y detectar hasta
        /// cierta distancia si hay suelo debajo del personaje. Si no lo hay es porque el personaje o está
        /// cayendo / saltando o porque la pendiente donde esté el personaje es demasiado inclinada
        /// 
        /// Nota: usar "maxDist = characterController.height / 2 * slopeForceRayLength" en caso de que
        /// el centro/pivote del objeto estuviera en el centro en lugar de en el suelo,
        /// siendo "slopeForceRayLength >= 1" un multiplicador de la longitud del rayo
        float maxDist = _slopeForceRayLength;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, maxDist)) { 
            //if (hit.normal != Vector3.up) {
            if (hit.normal.y <= 0.9) {
                Debug.Log(hit.normal);
                return true;
            }
            
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        float maxDist = _slopeForceRayLength;
        Vector3 p = transform.position;
        Gizmos.DrawLine(p, new Vector3(p.x, p.y - maxDist, p.z));
    }
}
