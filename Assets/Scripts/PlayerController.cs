using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private PowerUpSystem _powerUpSystem;
    private GameHUDController _hudController;
    private string[] _availableAbilities;
    private List<string> _collectedAbilitiesForHUD = new List<string>();
    private int _currentAbilityIndex = -1;

    [Header("Power Up Effects")]
    [SerializeField] private AudioClip _powerUpSound;
    private AudioSource _audioSource;

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        if (_playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on PlayerController!");
        }

        _powerUpSystem = GetComponent<PowerUpSystem>();
        if (_powerUpSystem == null)
        {
            Debug.LogError("PowerUpSystem component not found on PlayerController!");
        }

        _hudController = FindFirstObjectByType<GameHUDController>();
        if (_hudController == null)
        {
            Debug.LogError("GameHUDController not found in the scene!");
        }

        SpawnManager spawnManager = FindFirstObjectByType<SpawnManager>();
        if (spawnManager != null)
        {
            _availableAbilities = spawnManager.PowerUpPrefabNames;
        }
        else
        {
            Debug.LogError("SpawnManager not found!");
            _availableAbilities = new string[0];
        }
        UpdateHUDAbilityDisplay();

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        if (_playerMovement != null)
        {
            _playerMovement.Move(forwardInput, horizontalInput);
            if (_powerUpSystem != null)
            {
                _powerUpSystem.UpdatePowerBoostIndicatorPosition(transform.position);
                _powerUpSystem.UpdatePowerJumpIndicatorPosition(transform.position);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_powerUpSystem != null)
            {
                UseCurrentAbility();
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectPreviousAbility();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectNextAbility();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string collectedAbilityName = "";
        if (other.CompareTag("PowerBoostPowerUp"))
        {
            collectedAbilityName = "Power";
        }
        else if (other.CompareTag("RocketPowerUp"))
        {
            collectedAbilityName = "Rocket";
        }
        else if (other.CompareTag("PowerJumpPowerUp"))
        {
            collectedAbilityName = "Power Jump";
        }
        else if (other.CompareTag("InvisibilityPowerUp"))
        {
            collectedAbilityName = "Invisibility";
        }

        if (!string.IsNullOrEmpty(collectedAbilityName))
        {
            SoundOnPowerPickUp();

            if (_powerUpSystem != null)
            {
                _powerUpSystem.PowerUpCollected(collectedAbilityName);
                if (!_collectedAbilitiesForHUD.Contains(collectedAbilityName))
                {
                    _collectedAbilitiesForHUD.Add(collectedAbilityName);
                    if (_collectedAbilitiesForHUD.Count == 1)
                    {
                        _currentAbilityIndex = 0;
                    }
                    else if (_currentAbilityIndex == -1)
                    {
                        _currentAbilityIndex = 0;
                    }
                }
                UpdateHUDAbilityDisplay();
                _hudController.DisplayAbilityHelper(_collectedAbilitiesForHUD.Count);
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
                _powerUpSystem.ApplyPowerBoostEffect(enemyRigidBody, transform.position);

                //Apply force to enemy based on Push Resistance
                float pushForce = 100f;
                float resistanceFactor = enemyBase.PushResistance;

                // Apply force, scaled by push resistance.
                enemyRigidBody.AddForce(transform.forward * pushForce / resistanceFactor);
            }
        }
    }

    void UseCurrentAbility()
    {
        if (_currentAbilityIndex != -1 && _collectedAbilitiesForHUD.Count > 0 && _currentAbilityIndex < _collectedAbilitiesForHUD.Count)
        {
            string abilityToUse = _collectedAbilitiesForHUD[_currentAbilityIndex];

            if (_powerUpSystem != null && _powerUpSystem.GetPowerUpCount(abilityToUse) > 0)
            {
                Debug.Log($"Attempting to use ability: {abilityToUse}");

                if (abilityToUse == "Rocket")
                {
                    _powerUpSystem.ActivateRocket();
                }
                else if (abilityToUse == "Power Jump")
                {
                    _powerUpSystem.ActivatePowerJump();
                }
                else if (abilityToUse == "Invisibility")
                {
                    _powerUpSystem.ActivateInvisibility();
                }
                else if (abilityToUse == "Power")
                {
                    _powerUpSystem.ActivatePowerBoost();
                }
            }
            else
            {
                Debug.Log($"No {abilityToUse} available to use.");
                int indexToRemove = _collectedAbilitiesForHUD.IndexOf(abilityToUse);
                if (indexToRemove != -1)
                {
                    _collectedAbilitiesForHUD.RemoveAt(indexToRemove);
                    if (_collectedAbilitiesForHUD.Count == 0)
                    {
                        _currentAbilityIndex = -1;
                    }
                    else if (_currentAbilityIndex >= _collectedAbilitiesForHUD.Count)
                    {
                        _currentAbilityIndex = _collectedAbilitiesForHUD.Count - 1;
                    }
                }
                else if (_collectedAbilitiesForHUD.Count > 0 && _currentAbilityIndex >= _collectedAbilitiesForHUD.Count)
                {
                    _currentAbilityIndex = _collectedAbilitiesForHUD.Count - 1;
                }
                else if (_collectedAbilitiesForHUD.Count == 0)
                {
                    _currentAbilityIndex = -1;
                }
                UpdateHUDAbilityDisplay();
            }
        }
    }

    public void AbilityExpired(string abilityName)
    {
        if (_collectedAbilitiesForHUD.Contains(abilityName))
        {
            if (_powerUpSystem != null && _powerUpSystem.GetPowerUpCount(abilityName) <= 0)
            {
                int indexToRemove = _collectedAbilitiesForHUD.IndexOf(abilityName);
                _collectedAbilitiesForHUD.RemoveAt(indexToRemove);

                if (_collectedAbilitiesForHUD.Count == 0)
                {
                    _currentAbilityIndex = -1;
                }
                else if (_currentAbilityIndex >= _collectedAbilitiesForHUD.Count)
                {
                    _currentAbilityIndex = _collectedAbilitiesForHUD.Count - 1;
                }
                else if (indexToRemove <= _currentAbilityIndex && _currentAbilityIndex > 0)
                {
                    _currentAbilityIndex--;
                }
                else if (_currentAbilityIndex >= _collectedAbilitiesForHUD.Count)
                {
                    _currentAbilityIndex = 0;
                }
            }
        }
        UpdateHUDAbilityDisplay();
    }

    void SelectPreviousAbility()
    {
        if (_collectedAbilitiesForHUD.Count > 0)
        {
            _currentAbilityIndex--;
            if (_currentAbilityIndex < 0)
            {
                _currentAbilityIndex = _collectedAbilitiesForHUD.Count - 1;
            }
            UpdateHUDAbilityDisplay();
        }
    }
    void SelectNextAbility()
    {
        if (_collectedAbilitiesForHUD.Count > 0)
        {
            _currentAbilityIndex++;
            if (_currentAbilityIndex >= _collectedAbilitiesForHUD.Count)
            {
                _currentAbilityIndex = 0;
            }
            UpdateHUDAbilityDisplay();
        }
    }

    public void UpdateHUDAbilityDisplay()
    {
        if (_hudController != null && _collectedAbilitiesForHUD.Count > 0 && _currentAbilityIndex != -1 && _currentAbilityIndex < _collectedAbilitiesForHUD.Count && _powerUpSystem != null)
        {
            string currentAbilityName = _collectedAbilitiesForHUD[_currentAbilityIndex];
            int currentAbilityCount = _powerUpSystem.GetPowerUpCount(currentAbilityName);
            _hudController.UpdateAbilityDisplay(currentAbilityName, currentAbilityCount);
        }
        else if (_hudController != null)
        {
            _hudController.ClearAbilityDisplay();
        }
    }

    public int GetCurrentAbilityIndex()
    {
        return _currentAbilityIndex;
    }

    public string[] GetAvailableAbilities()
    {
        return _availableAbilities;
    }

    void SoundOnPowerPickUp()
    {
        if (_powerUpSound != null && GameSettings.Instance != null && GameSettings.Instance.IsSoundEnabled)
        {
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.PlayOneShot(_powerUpSound, 0.75f);
        }
    }
}
