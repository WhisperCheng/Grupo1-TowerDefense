using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine.InputSystem;
using UnityEngine;

public class MovPersonaje2D : MonoBehaviour
{
    /*[Header("Giro")]
    [SerializeField] private float _turnSpeed = 40.0f; // La velocidad de giro
    [Range(0f, 60f)]
    [SerializeField] private float _smoothTurnSpeed = 4f;*/

    [Header("Animaciones")]
    private CharacterController _characterController;
    [SerializeField] private Animator _animatorController;

    [Header("Gravedad")]
    [SerializeField] private float _velocidadGravedad = 1f;
    [SerializeField] private float _gravedadMagnitud = -9.81f;
    [SerializeField] private float _gravedadMultiplicador = 1f;

    [Header("Movimiento")]
    [SerializeField] private float _acceleration = 2.0f;
    //public float deceleration = 2.0f;
    [SerializeField] private float _maximumWalkVelocity = 4f;
    [SerializeField] private float _maximumRunVelocity = 10f;
    private float _currentMaxVelocity = 0;

    [Header("Suavizado y movimiento en pendientes")]
    [SerializeField] private float _smoothingMoveSpeed = 0.2f;
    [SerializeField] private float _slopeForce;
    [SerializeField] private float _slopeForceRayLength;
    public Transform orientation;



    private Vector3 _movCharacter;
    private float _velocityZ = 0.0f;
    private float _velocityX = 0.0f;
    private int _velocityZHash;
    private int _velocityXHash;

    PlayerInput playerInput;
    private Vector2 _moveDirection;
    private Vector2 _smoothedMoveDirection;
    private Vector2 _lookDirection;
    private float _oldLookDirection = 0f;
    private Vector2 _smoothedMoveSpeed;
    private int sueloMask = 1 << 6;
    
    //float rotateYAngle = 0f;

    void Start()
    {
        // Get the Character Controller on the player
        _characterController = GetComponent<CharacterController>();
        //animatorController = GetComponent<Animator>();
        _velocityZHash = Animator.StringToHash("VelZ");
        _velocityXHash = Animator.StringToHash("VelX");
        _currentMaxVelocity = _maximumWalkVelocity;

        playerInput = GetComponent<PlayerInput>();
    }
    void FixedUpdate()
    {

    }


    void Update()
    {
        MoveCharacter();
    }

    // Físicas y movimientos del personaje
    private void MoveCharacter()
    {
        _moveDirection = playerInput.actions["Move"].ReadValue<Vector2>();
        _lookDirection = playerInput.actions["Look"].ReadValue<Vector2>();

        /*
        // TODO (opcional/si da tiempo):
        /// Si el jugador no está tocando el suelo, se deshabilita temporalmente el control
        ///del movimiento horizontal
        
        RaycastHit hit;
        float radius = _characterController.radius / 2;_smoothTurnSpeed
        // Desde que el personaje en cierta medida se despegue del suelo, será considerado como que está en el aire
        //bool onAir = !Physics.Raycast(transform.position, -transform.up, 1);
        bool onAir = Physics.OverlapSphere(transform.position, _characterController.radius, sueloMask).Length == 0;
        if (onAir)
        {
            Debug.Log("sd " + transform.up);
            _moveDirection = Vector2.zero;
        }*/

        /// MOVIMIENTO
        // Suavizar el movimiento del vector moveDirection
        // Si empieza a correr el movimiento de suavizado será ligeramente menor
        bool runPressed = playerInput.actions["Correr"].ReadValue<float>() > 0 ? true : false;
        _smoothedMoveDirection = Vector2.SmoothDamp(_smoothedMoveDirection, _moveDirection,
            ref _smoothedMoveSpeed, _smoothingMoveSpeed * (runPressed ? 1f : 1.4f));

        _movCharacter = orientation.forward * _smoothedMoveDirection.y + orientation.right * //
            _smoothedMoveDirection.x;
        ApplyGravity();

        // Controlando si intenta correr y suavizar el movimiento de correr
        float currentRawMaxVelocity = runPressed ? _maximumRunVelocity : _maximumWalkVelocity;
        _currentMaxVelocity = Mathf.Lerp(_currentMaxVelocity,
            currentRawMaxVelocity, Time.deltaTime * _acceleration);

        // Mover personaje
        Vector3 mov = new Vector3(_movCharacter.x * _currentMaxVelocity, _movCharacter.y,
            _movCharacter.z * _currentMaxVelocity);
        Vector3 slopeFix = Vector3.zero;
        
        /// Si el jugador se está moviendo y está en una rampa, se aplica un movimiento correctivo para
        /// que pueda caminar o deslizarse por ella correctamente si está dentro del ángulo de poder caminar por rampas
        if ((_movCharacter.x != 0 || _movCharacter.z != 0) && OnSlope())
        {
            slopeFix = Vector3.down * _slopeForce * Time.deltaTime;
        }

        _characterController.Move((mov + slopeFix) * Time.deltaTime);

        /// SUAVIZADO DE TRANSICIONES
        float smoothAmount = (runPressed ? 1.3f : 1f);
        _velocityX = Mathf.Lerp(_velocityX,
            _moveDirection.x * currentRawMaxVelocity, Time.deltaTime * _acceleration * smoothAmount);
        _velocityZ = Mathf.Lerp(_velocityZ,
            _moveDirection.y * currentRawMaxVelocity, Time.deltaTime * _acceleration * smoothAmount);

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
    private void ApplyGravity()
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
            if (hit.normal.y <= 0.97) {
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
