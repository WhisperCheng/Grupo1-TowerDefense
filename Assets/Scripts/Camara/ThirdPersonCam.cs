using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Referencias de Cámara")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public CinemachineFreeLook freelookCamera;

    [Header("Giro de cámara")]
    [Range(0f, 80f)]
    [SerializeField] public float _turnSpeed = 30f;
    [Range(0f, 60f)]
    [SerializeField] public float _smoothCameraTurnSpeed = 4f;
    [Range(0.1f, 10f)]
    [SerializeField] private float _smoothPlayerTurnSpeed = 4f;

    private Vector2 _moveDirection;
    private Vector2 _lookDirection;
    private Vector2 _oldLookDirection;

    public static ThirdPersonCam instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateOrientation();
        RotatePlayerObj();
    }

    private void RotateOrientation()
    {
        // Leer input para luego apuntar a la dirección deseada
        PlayerInput playerInput = GameManager.Instance.playerControls;
        _lookDirection = playerInput.actions["Look"].ReadValue<Vector2>();

        // suavizar rotación continuamente
        _lookDirection.x = Mathf.Lerp(_oldLookDirection.x, _lookDirection.x, (60.1f - _smoothCameraTurnSpeed) * Time.smoothDeltaTime);
        // Rotamos en el eje Y, se rota suavizado
        if (_lookDirection.x != 0) // si se está moviendo el ratón se ejecuta código
        {
            float addRotation = _lookDirection.x * (_turnSpeed * Time.smoothDeltaTime);
            orientation.transform.Rotate(0, addRotation, 0); // rotación con suavizado
            _oldLookDirection.x = _lookDirection.x;
        }
    }

    private void RotatePlayerObj()
    {
        PlayerInput playerInput = GameManager.Instance.playerControls;
        _moveDirection = playerInput.actions["Move"].ReadValue<Vector2>();
        if (_moveDirection != Vector2.zero)
        {
            float verticalInput = _moveDirection.y;
            float horizontalInput = _moveDirection.x;

            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.smoothDeltaTime * _smoothPlayerTurnSpeed);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 0);
        Gizmos.DrawLine(orientation.transform.position, orientation.forward);
        
    }
}
