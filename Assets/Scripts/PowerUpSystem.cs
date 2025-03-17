using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerUpSystem : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private Dictionary<string, int> _powerUpInventory = new Dictionary<string, int>();
    private PlayerController _playerController;
    private AudioSource _audioSource;

    [Header("Power Powerup")]
    [SerializeField] private float _powerBoostStrength = 15.0f;
    [SerializeField] private float _powerBoostDuration = 15f;
    [SerializeField] private GameObject _powerBoostIndicator;
    [SerializeField] private AudioClip _powerBoostAudioEffect;
    private bool _isPowerBoostActive = false;
    private Vector3 _powerBoostIndicatorOffset = new Vector3(0, -0.5f, 0);
    public float PowerBoostRemainingTime { get; private set; }

    [Header("Rocket Powerup")]
    [SerializeField] private GameObject _rocketPrefab;
    [SerializeField] private Transform _rocketSpawnPoint;

    [Header("Power Jump Powerup")]
    [SerializeField] private float _powerJumpDuration = 20f;
    [SerializeField] private GameObject _powerJumpIndicator;
    [SerializeField] private AudioClip _powerJumpAudioEffect;
    private Vector3 _powerJumpIndicatorOffset = new Vector3(0, -0.5f, 0);
    private bool _isPowerJumpPowerUpActive = false;
    public float PowerJumpRemainingTime { get; private set; }

    [Header("Invisibility Powerup")]
    [SerializeField] private float _invisibilityDuration = 10f;
    [SerializeField] private Material _transparentPlayerMaterial;
    private Material _originalPlayerMaterial;
    private bool _isInvisibilityPowerUpActive = false;
    public float InvisibilityRemainingTime { get; private set; }

    [Header("Visual Effects")]
    [SerializeField] private Renderer _playerRenderer;
    private Color _originalColor;
    private int _originalLayer;
    [SerializeField] private string _invisibilityLayerName = "InvisiblePlayer";

    public bool HasPowerBoost
    {
        get { return _isPowerBoostActive; }
        private set { _isPowerBoostActive = value; }
    }

    public float PowerBoostDuration
    {
        get { return _powerBoostDuration; }
        private set { _powerBoostDuration = value; }
    }

    public GameObject PowerBoostIndicator
    {
        get { return _powerBoostIndicator; }
        private set { _powerBoostIndicator = value; }
    }

    public float PowerBoostStrength
    {
        get { return _powerBoostStrength; }
        private set { _powerBoostStrength = value; }
    }

    public bool PowerJumpPowerUpActive
    {
        get { return _isPowerJumpPowerUpActive; }
    }

    public float PowerJumpDuration
    {
        get { return _powerJumpDuration; }
    }

    public bool InvisibilityPowerUpActive
    {
        get { return _isInvisibilityPowerUpActive; }
    }

    public float InvisibilityPowerUpDuration
    {
        get { return _invisibilityDuration; }
    }

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        if (_playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on PowerUpSystem! Ensure PowerUpSystem and PlayerMovement are on the same GameObject.");
        }
        _playerController = GetComponent<PlayerController>();
        if (_playerController == null)
        {
            Debug.LogError("PlayerController component not found on PowerUpSystem!");
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
            _originalLayer = gameObject.layer;
            _originalColor = _playerRenderer.material.color;
            _originalPlayerMaterial = _playerRenderer.material;
        }
        else
        {
            Debug.LogError("Player Renderer is not assigned in PowerUpSystem!");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
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
        Debug.Log(">>> ActivatePowerBoost()");
        if (_powerUpInventory.ContainsKey("Power") && _powerUpInventory["Power"] > 0 && !_isPowerBoostActive)
        {
            _powerUpInventory["Power"]--;
            _isPowerBoostActive = true;
            UpdatePowerBoostIndicatorPosition(transform.position);
            Debug.Log("Power Boost Activated!");
            if (_powerBoostIndicator != null)
            {
                PowerBoostIndicator.gameObject.SetActive(true);
            }
            if (_playerController != null)
            {
                _playerController.AbilityExpired("Power");
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
        float timer = _powerBoostDuration;
        PowerBoostRemainingTime = timer;
        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
            PowerBoostRemainingTime = timer;
        }
        HasPowerBoost = false;

        if (_powerBoostIndicator != null)
        {
            PowerBoostIndicator.gameObject.SetActive(false);
        }
        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().material.color = _originalColor;
        }
        if (_playerController != null)
        {
            _playerController.AbilityExpired("Power");
        }
        PowerBoostRemainingTime = 0f;
    }

    public void ApplyPowerBoostEffect(Rigidbody enemyRb, Vector3 playerPosition)
    {
        if (_isPowerBoostActive)
        {
            Vector3 awayFromPlayer = enemyRb.transform.position - playerPosition;
            enemyRb.AddForce(awayFromPlayer * PowerBoostStrength, ForceMode.Impulse);
            PlayOneShotSound(_powerBoostAudioEffect);
        }
    }

    public void UpdatePowerBoostIndicatorPosition(Vector3 playerPosition)
    {
        if (_isPowerBoostActive && _powerBoostIndicator != null)
        {
            Vector3 indicatorPosition = playerPosition + _powerBoostIndicatorOffset;
            PowerBoostIndicator.transform.position = indicatorPosition;
        }
    }

    public void ActivateRocket()
    {
        if (_powerUpInventory.ContainsKey("Rocket") && _powerUpInventory["Rocket"] > 0)
        {
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
            _powerUpInventory["Rocket"]--;
            if (_playerController != null)
            {
                _playerController.AbilityExpired("Rocket");
            }
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
        if (_powerUpInventory.ContainsKey("Power Jump") && _powerUpInventory["Power Jump"] > 0 && !_isPowerJumpPowerUpActive)
        {
            _powerUpInventory["Power Jump"]--;
            _isPowerJumpPowerUpActive = true;
            _playerMovement.IsPowerJumpActive = true;
            Debug.Log("Power Jump Activated!");
            if (_playerController != null)
            {
                _playerController.AbilityExpired("Power Jump");
            }
            if (_powerJumpIndicator != null)
            {
                _powerJumpIndicator.gameObject.SetActive(true);
                UpdatePowerJumpIndicatorPosition(transform.position);
            }
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
        float timer = _powerJumpDuration;
        PowerJumpRemainingTime = timer;
        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
            PowerJumpRemainingTime = timer;
        }
        _isPowerJumpPowerUpActive = false;
        _playerMovement.IsPowerJumpActive = false;
        Debug.Log("Power Jump PowerUp Expired.");
        if (_powerJumpIndicator != null)
        {
            _powerJumpIndicator.gameObject.SetActive(false);
        }
        PowerJumpRemainingTime = 0f;
    }

    public void PlayPowerJumpAudioEffect()
    {
        PlayOneShotSound(_powerJumpAudioEffect);
    }

    public void UpdatePowerJumpIndicatorPosition(Vector3 playerPosition)
    {
        if (_isPowerJumpPowerUpActive && _powerJumpIndicator != null)
        {
            Vector3 indicatorPosition = new Vector3(playerPosition.x, 0, playerPosition.z) + _powerJumpIndicatorOffset;
            _powerJumpIndicator.transform.position = indicatorPosition;
        }
    }

    public void ActivateInvisibility()
    {
        if (_powerUpInventory.ContainsKey("Invisibility") && _powerUpInventory["Invisibility"] > 0 && !_isInvisibilityPowerUpActive)
        {
            _powerUpInventory["Invisibility"]--;
            _isInvisibilityPowerUpActive = true;
            if (_playerController != null)
            {
                _playerController.AbilityExpired("Invisibility");
            }
            StartCoroutine(InvisibilityPowerUpCountdownRoutine());
        }
        else if (_isInvisibilityPowerUpActive)
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
        if (_playerRenderer != null && _transparentPlayerMaterial != null)
        {
            _playerRenderer.material = _transparentPlayerMaterial;

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

        float timer = _invisibilityDuration;
        InvisibilityRemainingTime = timer;
        while (timer > 0)
        {
            yield return null;
            timer -= Time.deltaTime;
            InvisibilityRemainingTime = timer;
        }

        if (_playerRenderer != null)
        {
            _playerRenderer.material = _originalPlayerMaterial;
            gameObject.layer = _originalLayer;
        }
        _isInvisibilityPowerUpActive = false;
        InvisibilityRemainingTime = 0f;
    }

    private Transform GetRandomEnemyTarget()
    {
        EnemyBase[] enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        if (enemies.Length > 0)
        {
            EnemyBase[] validEnemies = enemies.Where(enemy => enemy != null && enemy.gameObject != null).ToArray();
            List<EnemyBase> validEnemiesList = new List<EnemyBase>();
            foreach (EnemyBase enemy in validEnemies)
            {
                if (enemy.transform.position.x <= 9f && enemy.transform.position.x >= -9f &&
                enemy.transform.position.y <= 9f && enemy.transform.position.y >= -9f)
                {
                    validEnemiesList.Add(enemy);
                }
            }
            if (validEnemiesList.Count > 0)
            {
                int randomIndex = Random.Range(0, validEnemiesList.Count);
                return validEnemiesList[randomIndex].transform;
            }
        }
        return null;
    }

    void PlayOneShotSound(AudioClip clip)
    {
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(clip, 0.75f);
        }
    }
}
