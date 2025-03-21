using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    private PowerUpSystem _powerUpSystem;
    private Rigidbody _playerRb;
    private GameObject _focalPoint;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 25.0f;
    [SerializeField] private float _groundCheckDistance = 1.2f;
    private bool _isPowerJumpActive = false;
    private bool _isGrounded = true;

    public bool IsPowerJumpActive
    {
        get { return _isPowerJumpActive; }
        set { _isPowerJumpActive = value; }
    }

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    private void Awake()
    {
        _playerRb = GetComponent<Rigidbody>();

        _focalPoint = GameObject.FindGameObjectWithTag("FocalPoint");
        if (_focalPoint == null)
        {
            Debug.LogError("Focal Point not found! Make sure your Focal Point GameObject is tagged 'FocalPoint'.");
        }

        _powerUpSystem = GetComponent<PowerUpSystem>();
        if (_powerUpSystem == null)
        {
            Debug.LogError("PowerUpSystem component not found on PlayerController!");
        }
    }

    private void Update()
    {
        if (_isPowerJumpActive)
        {
            HandleJumpInput();
        }
        CheckGround();

        if (transform.position.y < -10f)
        {
            SpawnManager spawnManager = FindFirstObjectByType<SpawnManager>();
            if (spawnManager != null)
            {
                spawnManager.GameOver();
            }
            else
            {
                Debug.LogError("SpawnManager not found! Cannot trigger Game Over.");
            }
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _playerRb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _isGrounded = false;
            if (_powerUpSystem != null)
            {
                _powerUpSystem.PlayPowerJumpAudioEffect();
            }
        }
    }

    private void CheckGround()
    {
        RaycastHit hit;
        bool raycastHit = Physics.Raycast(transform.position, Vector3.down, out hit, _groundCheckDistance);
        _isGrounded = raycastHit && !hit.collider.isTrigger;
    }

    public void Move(float forwardInput, float horizontalInput)
    {

        if (_focalPoint != null)
        {
            Vector3 movementDirection = Vector3.zero;
            if (GameSettings.Instance != null && GameSettings.Instance.UseCameraRelativeMovement)
            {
                //Camera-Relative Movement
                movementDirection += _focalPoint.transform.forward * forwardInput;
                _playerRb.AddForce(movementDirection * _speed);
            }
            else
            {
                //Direct Input Movement (Camera-Relative Direction)
                Transform cameraTransform = Camera.main.transform;
                Vector3 cameraForward = cameraTransform.forward;
                Vector3 cameraRight = cameraTransform.right;

                cameraForward.y = 0f;
                cameraRight.y = 0f;
                cameraForward.Normalize();
                cameraRight.Normalize();

                Vector3 forwardMove = cameraForward * forwardInput;
                Vector3 rightMove = cameraRight * horizontalInput;

                movementDirection += forwardMove;
                movementDirection += rightMove;

                _playerRb.AddForce(movementDirection * _speed);
            }
        }
    }
}
