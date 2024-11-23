using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Referencias de Cámara")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    //[SerializeField] private float rotationSpeed = 40.0f; // La velocidad de giro

    [Header("Giro de cámara")]
    [SerializeField] private float _turnSpeed = 40f;
    [Range(0f, 60f)]
    [SerializeField] private float _smoothTurnSpeed = 4f;

    [Header("Controles")]
    public PlayerInput playerInput;

    private Vector2 _moveDirection;
    private Vector2 _lookDirection;
    private Vector2 _oldLookDirection;
    //private Vector2 _smoothedMoveSpeed; // sin uso, funciona como ref

    // Start is called before the first frame update
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
        // Apuntar hacia delante
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        // Leer input para luego apuntar a la dirección deseada
        _lookDirection = playerInput.actions["Look"].ReadValue<Vector2>();


        // Rotamos en el eje Y, se rota suavizado
        if (_lookDirection.x != 0) // si se está moviendo el ratón se ejecuta código
        {
            _lookDirection.x = Mathf.Lerp(_oldLookDirection.x, _lookDirection.x, (60.1f - _smoothTurnSpeed) * Time.deltaTime); // suavizar rotación
            float addRotation = _lookDirection.x * _turnSpeed * Time.deltaTime;
            orientation.transform.Rotate(0, addRotation, 0); // rotación con suavizado
            _oldLookDirection.x = _lookDirection.x;
        }
    }

    private void RotatePlayerObj()
    {
        _moveDirection = playerInput.actions["Move"].ReadValue<Vector2>();
        if (_moveDirection != Vector2.zero)
        {
            float verticalInput = _moveDirection.y;
            float horizontalInput = _moveDirection.x;

            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
            if (inputDir != Vector3.zero)
            {
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * _smoothTurnSpeed);
            }
        }
    }
}
