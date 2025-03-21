using UnityEngine;
using UnityEngine.SceneManagement;

public class SkyDomeRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5.0f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameMain" && GameSettings.Instance != null && !GameSettings.Instance.UseCameraRelativeMovement)
        {
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.y += rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        }
    }
}
