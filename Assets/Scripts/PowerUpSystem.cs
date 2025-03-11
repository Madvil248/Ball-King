using System.Collections;
using System.Linq;
using UnityEngine;

public class PowerUpSystem : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private bool _hasPowerUp = false;
    private Vector3 _powerUpOffset = new Vector3(0, -0.5f, 0);

    [Header("Power Powerup")]
    [SerializeField] private float _powerUpStrength = 15.0f;
    [SerializeField] private float _powerUpDuration = 7f;
    [SerializeField] private GameObject _powerUpIndicator;

    [Header("Rocket Powerup")]
    [SerializeField] private GameObject _rocketPrefab;
    [SerializeField] private Transform _rocketSpawnPoint;
    private bool _hasRocketPowerUp = false;

    [Header("Power Jump Powerup")]
    [SerializeField] private float _powerJumpDuration = 30f;
    private bool _isPowerJumpPowerUpActive = false;

    public bool HasPowerUp
    {
        get { return _hasPowerUp; }
        private set { _hasPowerUp = value; }
    }

    public bool HasRocketPowerUp
    {
        get { return _hasRocketPowerUp; }
        private set { _hasRocketPowerUp = value; }
    }

    public bool HasPowerJumpPowerUp
    {
        get { return _isPowerJumpPowerUpActive; }
        private set { _isPowerJumpPowerUpActive = value; }
    }

    public GameObject PowerUpIndicator
    {
        get { return _powerUpIndicator; }
        private set { _powerUpIndicator = value; }
    }

    public float PowerUpStrength
    {
        get { return _powerUpStrength; }
        private set { _powerUpStrength = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_powerUpIndicator == null)
        {
            Debug.LogError("Power-Up Indicator is not assigned in PowerUpSystem!");
        }
        else
        {
            _powerUpIndicator.gameObject.SetActive(false);
        }

        if (_rocketSpawnPoint == null)
        {
            Debug.LogError("Rocket Spawn Point is not assigned on PowerUpSystem! Please create and assign one.");
        }
        _playerMovement = GetComponent<PlayerMovement>();
        if (_playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on PowerUpSystem! Ensure PowerUpSystem and PlayerMovement are on the same GameObject.");
        }
    }

    // For Speed Boost PowerUp collect
    public void CollectPowerUp()
    {
        HasPowerUp = true;
        Debug.Log("Speed Boost PowerUp Collected! Press Fire button to activate.");
        Debug.Log("HasPowerUp set to: " + HasPowerUp);
        if (_powerUpIndicator != null)
        {
            PowerUpIndicator.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("PowerUpSystem: _powerUpIndicator IS NULL!  Assignment problem?"); // Added Debug.Log - Check for null assignment
        }
    }

    public void CollectRocketPowerUp()
    {
        HasRocketPowerUp = true;
        Debug.Log("Rocket PowerUp Collected! Press Fire button to launch rocket.");
        Debug.Log("HasRocketPowerUp set to: " + HasRocketPowerUp);
    }

    public void CollectPowerJumpPowerUp()
    {
        HasPowerJumpPowerUp = true;
        Debug.Log("Power Jump PowerUp Collected! Press F key to activate Jump.");
    }

    private IEnumerator PowerUpCountdownRoutine()
    {
        yield return new WaitForSeconds(_powerUpDuration);
        HasPowerUp = false;

        if (_powerUpIndicator != null)
        {
            PowerUpIndicator.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("PowerUpSystem: _powerUpIndicator IS NULL in CountdownRoutine!  Assignment problem?"); // Added Debug.Log - Check for null assignment
        }
    }

    private IEnumerator PowerJumpPowerUpCountdownRoutine()
    {
        _isPowerJumpPowerUpActive = true;
        _playerMovement.IsPowerJumpActive = true;
        Debug.Log("Power Jump Activated!");

        yield return new WaitForSeconds(_powerJumpDuration);

        _isPowerJumpPowerUpActive = false;
        _playerMovement.IsPowerJumpActive = false;
        Debug.Log("Power Jump PowerUp Expired.");
    }

    public void ApplyPowerUpEffect(Rigidbody enemyRb, Vector3 playerPosition)
    {
        if (HasPowerUp)
        {
            Vector3 awayFromPlayer = enemyRb.transform.position - playerPosition;
            enemyRb.AddForce(awayFromPlayer * PowerUpStrength, ForceMode.Impulse);
        }
    }

    public void UpdatePowerUpIndicatorPosition(Vector3 playerPosition)
    {
        if (_powerUpIndicator != null)
        {
            Vector3 indicatorPosition = playerPosition + _powerUpOffset;
            PowerUpIndicator.transform.position = indicatorPosition;
        }
    }

    private void LaunchRocket()
    {
        if (!HasRocketPowerUp)
        {
            Debug.LogWarning("Trying to launch rocket but Rocket PowerUp is not collected!");
            return;
        }

        if (_rocketPrefab == null || _rocketSpawnPoint == null)
        {
            Debug.LogError("Rocket Prefab or Rocket Spawn Point not assigned in PowerUpSystem!");
            return;
        }

        Transform targetEnemy = GetRandomEnemyTarget();

        if (targetEnemy != null)
        {
            Debug.Log("Rocket Launched! Target Acquired: " + targetEnemy.name);
            GameObject rocketInstance = Instantiate(_rocketPrefab, _rocketSpawnPoint.position, _rocketSpawnPoint.rotation);
            RocketProjectile rocketProjectile = rocketInstance.GetComponent<RocketProjectile>();
            if (rocketProjectile != null)
            {
                rocketProjectile.SetTarget(targetEnemy);
            }
            else
            {
                Debug.LogError("Rocket Prefab is missing RocketProjectile script!");
                Destroy(rocketInstance);
            }
        }
        else
        {
            Debug.LogWarning("No enemies found to target for Rocket Powerup!");
            // Optionally, you could play a "no target" sound or visual feedback here
        }
        HasRocketPowerUp = false; // Reset Rocket Powerup flag after launching - one-time use per pickup
    }

    private Transform GetRandomEnemyTarget()
    {
        EnemyBase[] enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        if (enemies.Length > 0)
        {
            EnemyBase[] validEnemies = enemies.Where(enemy => enemy != null && enemy.gameObject != null).ToArray();
            if (validEnemies.Length > 0)
            {
                int randomIndex = Random.Range(0, validEnemies.Length);
                return validEnemies[randomIndex].transform;
            }
        }
        return null;
    }
    public void ActivatePowerUp()
    {
        Debug.Log("ActivatePowerUp() function called!");
        if (HasRocketPowerUp)
        {
            Debug.Log("HasRocketPowerUp is TRUE");
            LaunchRocket();
        }
        else if (HasPowerUp)
        {
            Debug.Log("HasPowerUp is TRUE");
            StartCoroutine(PowerUpCountdownRoutine());
            GetComponent<Renderer>().material.color = Color.yellow;
            Debug.Log("Speed Boost Activated!");
        }
        else if (HasPowerJumpPowerUp)
        {
            Debug.Log("HasPowerJumpPowerUp is TRUE");
            StartCoroutine(PowerJumpPowerUpCountdownRoutine());
        }
        else
        {
            Debug.Log("No powerup active to activate.");
        }
    }
}
