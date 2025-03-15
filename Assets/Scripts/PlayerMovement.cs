using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    public bool useCameraRelativeMovement = false;
    private Rigidbody _playerRb;
    private GameObject _focalPoint;
    private RotateCamera _rotateCamera;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 8.0f;
    [SerializeField] private float _jumpExplosionRadius = 7f;
    [SerializeField] private float _jumpExplosionForce = 500f;
    [SerializeField] private float _groundCheckDistance = 0.8f;
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
        _rotateCamera = _focalPoint.GetComponent<RotateCamera>();

        if (_focalPoint == null)
        {
            Debug.LogError("Focal Point not found! Make sure your Focal Point GameObject is tagged 'FocalPoint'.");
        }

        if (_rotateCamera == null)
        {
            Debug.LogError("RotateCamera component not found on Main Camera!");
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
        }
    }

    private void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, _groundCheckDistance))
        {
            if (!hit.collider.isTrigger)
            {
                _isGrounded = true;
            }
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_isGrounded && _isPowerJumpActive)
        {
            ExplosionOnLand();
            _isGrounded = true;
        }
    }

    private void ExplosionOnLand()
    {
        Debug.Log("Power Jump Landing Explosion!");
        Collider[] colliders = Physics.OverlapSphere(transform.position, _jumpExplosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null && nearbyObject.CompareTag("Enemy"))
            {
                rb.AddExplosionForce(_jumpExplosionForce, transform.position, _jumpExplosionRadius, 0f, ForceMode.Impulse);
            }
        }
    }

    public void Move(float forwardInput, float horizontalInput)
    {

        if (_focalPoint != null && _rotateCamera != null)
        {
            Vector3 movementDirection = Vector3.zero;
            if (useCameraRelativeMovement)
            {
                //Camera-Relative Movement
                _rotateCamera.cameraRotationEnabled = true;
                movementDirection += _focalPoint.transform.forward * forwardInput;
                _playerRb.AddForce(movementDirection * _speed);
            }
            else
            {
                //Direct Input Movement (Camera-Relative Direction)
                _rotateCamera.cameraRotationEnabled = false;

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
