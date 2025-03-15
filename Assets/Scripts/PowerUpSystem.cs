using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerUpSystem : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private bool _isPowerBoostActive = false;
    private Vector3 _powerBoostIndicatorOffset = new Vector3(0, -0.5f, 0);
    private Dictionary<string, int> _powerUpInventory = new Dictionary<string, int>();

    [Header("Power Powerup")]
    [SerializeField] private float _powerBoostStrength = 15.0f;
    [SerializeField] private float _powerUpDuration = 7f;
    [SerializeField] private GameObject _powerBoostIndicator;

    [Header("Rocket Powerup")]
    [SerializeField] private GameObject _rocketPrefab;
    [SerializeField] private Transform _rocketSpawnPoint;

    [Header("Power Jump Powerup")]
    [SerializeField] private float _powerJumpDuration = 30f;
    private bool _isPowerJumpPowerUpActive = false;

    [Header("Invisibility Powerup")]
    [SerializeField] private float _invisibilityDuration = 20f;
    private bool _hasInvisibilityPowerUp = false;

    [Header("Visual Effects")]
    [SerializeField] private Renderer _playerRenderer;
    private Color _originalColor;
    private int _originalLayer;
    [SerializeField] private string _invisibilityLayerName = "InvisiblePlayer";

    public bool HasPowerPowerUp
    {
        get { return _isPowerBoostActive; }
        private set { _isPowerBoostActive = value; }
    }

    public GameObject PowerUpIndicator
    {
        get { return _powerBoostIndicator; }
        private set { _powerBoostIndicator = value; }
    }

    public float PowerBoostStrength
    {
        get { return _powerBoostStrength; }
        private set { _powerBoostStrength = value; }
    }

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        if (_playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on PowerUpSystem! Ensure PowerUpSystem and PlayerMovement are on the same GameObject.");
        }

        if (_powerBoostIndicator != null)
        {
            _powerBoostIndicator.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Power-Up Indicator is not assigned in PowerUpSystem!");
        }

        if (_rocketSpawnPoint == null)
        {
            Debug.LogError("Rocket Spawn Point is not assigned on PowerUpSystem! Please create and assign one.");
        }


        if (_playerRenderer != null)
        {
            _originalColor = _playerRenderer.material.color;
            _originalLayer = gameObject.layer;
        }
        else
        {
            Debug.LogError("Player Renderer is not assigned in PowerUpSystem!");
        }
    }

    public void PowerUpCollected(string powerUpName)
    {
        Debug.Log($"{powerUpName} PowerUp Collected by PowerUpSystem.");

        if (_powerUpInventory.ContainsKey(powerUpName))
        {
            _powerUpInventory[powerUpName]++;
        }
        else
        {
            _powerUpInventory.Add(powerUpName, 1);
        }
        Debug.Log($"PowerUpSystem Inventory: {string.Join(", ", _powerUpInventory.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
    }

    public int GetPowerUpCount(string powerUpName)
    {
        if (_powerUpInventory.ContainsKey(powerUpName))
        {
            return _powerUpInventory[powerUpName];
        }
        return 0;
    }

    public void ActivatePowerBoost()
    {
        if (_powerUpInventory.ContainsKey("Power") && _powerUpInventory["Power"] > 0 && !_isPowerBoostActive)
        {
            _powerUpInventory["Power"]--;
            _isPowerBoostActive = true;
            Debug.Log("Power Boost Activated!");
            if (_powerBoostIndicator != null)
            {
                PowerUpIndicator.gameObject.SetActive(true);
            }
            StartCoroutine(PowerBoostCountdownRoutine());
            if (GetComponent<Renderer>() != null)
            {
                GetComponent<Renderer>().material.color = Color.yellow;
            }
        }
        else if (_isPowerBoostActive)
        {
            Debug.Log("Speed Boost is already active.");
        }
        else
        {
            Debug.Log("No Speed Boost available.");
        }
    }

    private IEnumerator PowerBoostCountdownRoutine()
    {
        yield return new WaitForSeconds(_powerUpDuration);
        HasPowerPowerUp = false;

        if (_powerBoostIndicator != null)
        {
            PowerUpIndicator.gameObject.SetActive(false);
        }
        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().material.color = _originalColor;
        }
    }

    public void ApplyPowerBoostEffect(Rigidbody enemyRb, Vector3 playerPosition)
    {
        if (_isPowerBoostActive)
        {
            Vector3 awayFromPlayer = enemyRb.transform.position - playerPosition;
            enemyRb.AddForce(awayFromPlayer * PowerBoostStrength, ForceMode.Impulse);
        }
    }

    public void UpdatePowerBoostIndicatorPosition(Vector3 playerPosition)
    {
        if (_isPowerBoostActive && _powerBoostIndicator != null)
        {
            Vector3 indicatorPosition = playerPosition + _powerBoostIndicatorOffset;
            PowerUpIndicator.transform.position = indicatorPosition;
        }
    }

    public void ActivateRocket()
    {
        if (_powerUpInventory.ContainsKey("Rocket") && _powerUpInventory["Rocket"] > 0)
        {
            _powerUpInventory["Rocket"]--;
            LaunchRocket();
        }
        else
        {
            Debug.Log("No Rockets available.");
        }
    }

    private void LaunchRocket()
    {
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
    }

    public void ActivatePowerJump()
    {
        if (_powerUpInventory.ContainsKey("Power Jump") && _powerUpInventory["Power Jump"] > 0 && _isPowerJumpPowerUpActive)
        {
            _powerUpInventory["Power Jump"]--;
            _isPowerJumpPowerUpActive = true;
            _playerMovement.IsPowerJumpActive = true;
            Debug.Log("Power Jump Activated!");
            StartCoroutine(PowerJumpPowerUpCountdownRoutine());
        }
        else if (_isPowerJumpPowerUpActive)
        {
            Debug.Log("Power Jump is already active.");
        }
        else
        {
            Debug.Log("Power Jump is already active.");
        }
    }

    private IEnumerator PowerJumpPowerUpCountdownRoutine()
    {
        yield return new WaitForSeconds(_powerJumpDuration);

        _isPowerJumpPowerUpActive = false;
        _playerMovement.IsPowerJumpActive = false;
        Debug.Log("Power Jump PowerUp Expired.");
    }

    public void ActivateInvisibility()
    {
        if (_powerUpInventory.ContainsKey("Invisibility") && _powerUpInventory["Invisibility"] > 0 && !_hasInvisibilityPowerUp)
        {
            _powerUpInventory["Invisibility"]--;
            _hasInvisibilityPowerUp = true;
            StartCoroutine(InvisibilityPowerUpCountdownRoutine());
        }
        else if (_hasInvisibilityPowerUp)
        {
            Debug.Log("Invisibility is already active.");
        }
        else
        {
            Debug.Log("Invisibility is already active.");
        }
    }

    private IEnumerator InvisibilityPowerUpCountdownRoutine()
    {
        Debug.Log("Invisibility Activated!");
        if (_playerRenderer != null)
        {
            Color transparentColor = _originalColor;
            transparentColor.a = 0.6f;
            _playerRenderer.material.color = transparentColor;

            int invisibilityLayer = LayerMask.NameToLayer(_invisibilityLayerName);
            if (invisibilityLayer != -1)
            {
                gameObject.layer = invisibilityLayer;
            }
            else
            {
                Debug.LogError($"Layer '{_invisibilityLayerName}' not found! Make sure it exists in Layer settings.");
            }
        }
        yield return new WaitForSeconds(_invisibilityDuration);
        Debug.Log("Invisibility PowerUp Expired!");
        if (_playerRenderer != null)
        {
            // Revert to Original Color
            _playerRenderer.material.color = _originalColor;

            // Revert to Original Layer
            gameObject.layer = _originalLayer;
        }
        _hasInvisibilityPowerUp = false;
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
}
