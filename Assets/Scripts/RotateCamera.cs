using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50.0f;
    public bool cameraRotationEnabled = true; // ADD THIS LINE - Control camera rotation

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (cameraRotationEnabled)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.up, rotationSpeed * horizontalInput * Time.deltaTime);
        }
    }
}
