using UnityEngine;

public class MainMenuCameraRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 0.1f;
    [SerializeField] private float _rotationRange = 5f;
    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void Update()
    {
        float angle = Mathf.PingPong(Time.time * _rotationSpeed, _rotationRange * 2) - _rotationRange;
        transform.rotation = Quaternion.Euler(initialRotation.eulerAngles.x, initialRotation.eulerAngles.y + angle, initialRotation.eulerAngles.z);
    }
}
