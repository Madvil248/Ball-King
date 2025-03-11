using UnityEngine;

public class PowerIndicatorMovement : MonoBehaviour
{
    private float rotationSpeed = 100.0f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
