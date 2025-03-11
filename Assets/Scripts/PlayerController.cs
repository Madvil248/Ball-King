using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private PowerUpSystem _powerUpSystem;

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _powerUpSystem = GetComponent<PowerUpSystem>();

        if (_playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on PlayerController!");
        }
        if (_powerUpSystem == null)
        {
            Debug.LogError("PowerUpSystem component not found on PlayerController!");
        }
    }

    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        if (_playerMovement != null)
        {
            _playerMovement.Move(forwardInput, horizontalInput);
            _powerUpSystem.UpdatePowerUpIndicatorPosition(transform.position);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_powerUpSystem != null)
            {
                _powerUpSystem.ActivatePowerUp();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            if (_powerUpSystem != null)
            {
                _powerUpSystem.CollectPowerUp();
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("RocketPowerUp"))
        {
            if (_powerUpSystem != null)
            {
                _powerUpSystem.CollectRocketPowerUp();
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("PowerJumpPowerUp"))
        {
            if (_powerUpSystem != null)
            {
                _powerUpSystem.CollectPowerJumpPowerUp();
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("InvisibilityPowerUp"))
        {
            if (_powerUpSystem != null)
            {
                _powerUpSystem.CollectInvisibilityPowerUp();
            }
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidBody = collision.gameObject.GetComponent<Rigidbody>();
            EnemyBase enemyBase = collision.gameObject.GetComponent<EnemyBase>();

            if (enemyRigidBody != null && _powerUpSystem != null && enemyBase != null)
            {
                _powerUpSystem.ApplyPowerUpEffect(enemyRigidBody, transform.position);

                //Apply force to enemy based on Push Resistance
                float pushForce = 100f;
                float resistanceFactor = enemyBase.PushResistance;

                // Apply force, scaled by push resistance.
                enemyRigidBody.AddForce(transform.forward * pushForce / resistanceFactor);
            }
        }
    }
}
