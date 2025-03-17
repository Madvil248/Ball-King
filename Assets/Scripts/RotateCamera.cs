using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50.0f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameSettings.Instance != null && GameSettings.Instance.UseCameraRelativeMovement)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.up, rotationSpeed * horizontalInput * Time.deltaTime);
        }
    }
}
