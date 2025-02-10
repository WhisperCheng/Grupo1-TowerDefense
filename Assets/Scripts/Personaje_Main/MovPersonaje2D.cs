using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class MovPersonaje2D : MonoBehaviour
{

    [Header("Animaciones")]
    private CharacterController _characterController;
    [SerializeField] private Animator _animatorController;
    [SerializeField] private ParticleSystem _MoveParticles;
    [SerializeField] private ParticleSystem _MoveParticlesStars;
    [SerializeField] private float _ParticleEmission = 25f;
    [SerializeField] private float _ParticleEmissionStars = 25f;
    [SerializeField] private float _MovParticleStopTime = 1f;

    [Header("Controles")]
    [SerializeField] private PlayerInput playerInput;

    [Header("Gravedad")]
    [SerializeField] private float _velocidadGravedad = 1f;
    [SerializeField] private float _gravedadMagnitud = -9.81f;
    [SerializeField] private float _gravedadMultiplicador = 1f;

    [Header("Movimiento")]
    [SerializeField] private float _acceleration = 2.0f;
    [SerializeField] private float _maximumWalkVelocity = 4f;
    [SerializeField] private float _maximumRunVelocity = 10f;
    private float _currentMaxVelocity = 0;

    [Header("Suavizado y movimiento en pendientes")]
    [SerializeField] private float _smoothingMoveSpeed = 0.2f;
    [SerializeField] private float _slopeForce;
    [Range(0, 2f)]
    [SerializeField] private float _slopeForceRayLength;
    public Transform orientation;

    private bool _onGround;

    private Vector3 _movCharacter;
    private float _velocityZ = 0.0f;
    private float _velocityX = 0.0f;
    private float _velocityM = 0.0f;
    private int _velocityZHash;
    private int _velocityXHash;
    private int _velocityMHash;

    private Vector2 _moveDirection;
    private Vector2 _smoothedMoveDirection;
    private Vector2 _smoothedMoveSpeed;
    

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _velocityZHash = Animator.StringToHash("VelZ");
        _velocityXHash = Animator.StringToHash("VelX");
        _velocityMHash = Animator.StringToHash("VelM");
        _currentMaxVelocity = _maximumWalkVelocity;
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
        if ((_movCharacter.x != 0 || _movCharacter.z != 0) && OnSlope() && _onGround)
        {
            slopeFix = Vector3.down * _slopeForce * Time.deltaTime;
        }

        if(_characterController.enabled) // Solo se moverá el personaje si el componente está habilitado, evitando en caso contrario
        _characterController.Move((mov + slopeFix) * Time.deltaTime);                                           // warnings por consola

        /// SUAVIZADO DE TRANSICIONES
        float smoothAmount = (runPressed ? 1.3f : 1f);
        _velocityX = Mathf.Lerp(_velocityX,
            _moveDirection.x * currentRawMaxVelocity, Time.deltaTime * _acceleration * smoothAmount);
        _velocityZ = Mathf.Lerp(_velocityZ,
            _moveDirection.y * currentRawMaxVelocity, Time.deltaTime * _acceleration * smoothAmount);
        _velocityM = Mathf.Lerp(_velocityM,
           _moveDirection.magnitude * currentRawMaxVelocity, Time.deltaTime * _acceleration * smoothAmount);

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
        if (_velocityM < 0.001f && _velocityM > -0.001f)
        {
            _velocityM = 0f;
        }

        var particlescount = _MoveParticles.emission;
        var particlescountstars = _MoveParticlesStars.emission;

        if (_velocityM > _MovParticleStopTime)
        {
            particlescountstars.rateOverTime = _ParticleEmissionStars;
            particlescount.rateOverTime = _ParticleEmission;
        }
        else
        {
            particlescount.rateOverTime = 0f;
            particlescountstars.rateOverTime = 0f;
        }

        _animatorController.SetFloat(_velocityZHash, _velocityX);
        _animatorController.SetFloat(_velocityXHash, _velocityZ);
        _animatorController.SetFloat(_velocityMHash, _velocityM);
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
        RaycastHit hit = new RaycastHit();

        /// Usa la posición del personaje partiendo desde su base para hacer un raycast y detectar hasta
        /// cierta distancia si hay suelo debajo del personaje. Si no lo hay es porque el personaje o está
        /// cayendo / saltando o porque la pendiente donde esté el personaje es demasiado inclinada
        if (OnGround(hit))
        {
            float slopeNormalLimit = (86f / 90); // Si la perpendicular de la pendiente es
            if (hit.normal.y <= slopeNormalLimit) { // menor a 86 grados se retorna true
                return true;
            }
        }
        return false;
    }

    private bool OnGround(RaycastHit hit)
    {
        float maxDist = _slopeForceRayLength;
        bool result = Physics.Raycast(transform.position, Vector3.down, out hit, maxDist);

        // Se detectará al personaje en el suelo solo cuando la detección del raycast y la del
        // _characterController.isGrounded se cumplen, por lo que solo se cumplirán las condiciones
        // cuando el personaje toque suelo (ignorando el raycast del suelo cercano)
        if (result && _characterController.isGrounded)
        {
            _onGround = true;
        }
        if (!result && !_characterController.isGrounded) // Si el personaje deja de tocar el suelo, se cambia _onGround a false
            _onGround = false;

        return result;
    }

    private void OnDrawGizmos()
    {
        float maxDist = _slopeForceRayLength;
        Vector3 p = transform.position;
        Gizmos.DrawLine(p, new Vector3(p.x, p.y - maxDist, p.z));
    }
}
