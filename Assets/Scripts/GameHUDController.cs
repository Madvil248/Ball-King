using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameHUDController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _waveAnnounceText;
    [SerializeField] private TextMeshProUGUI _abilityDisplayText;
    [SerializeField] private TextMeshProUGUI _prevAbilityText;
    [SerializeField] private TextMeshProUGUI _nextAbilityText;
    [SerializeField] private TextMeshProUGUI _useAbilityText;

    [Header("Power-up Timers")]
    [SerializeField] private Image _powerBoostTimerImage;
    [SerializeField] private Image _powerJumpTimerImage;
    [SerializeField] private Image _InvisibilityTimerImage;

    [Header("Background Music")]
    [SerializeField] private AudioClip _gameBackgroundMusic;
    private AudioSource _audioSource;

    private int _currentScore = 0;
    private PowerUpSystem _powerUpSystem;

    void Start()
    {
        UpdateScore(_currentScore);
        ClearAbilityDisplay();

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        PlayBackgroundMusic();

        _powerUpSystem = FindFirstObjectByType<PowerUpSystem>();
        if (_powerUpSystem == null)
        {
            Debug.LogError("PowerUpSystem not found in the scene!");
        }

        if (_powerBoostTimerImage != null) _powerBoostTimerImage.gameObject.SetActive(false);
        if (_powerJumpTimerImage != null) _powerJumpTimerImage.gameObject.SetActive(false);
        if (_InvisibilityTimerImage != null) _InvisibilityTimerImage.gameObject.SetActive(false);
    }


    void Update()
    {
        UpdateAbilityTimers();
    }

    public void UpdateScore(int score)
    {
        _currentScore = score;
        if (_scoreText != null)
        {
            _scoreText.text = "Score " + _currentScore.ToString();
        }
    }

    public void AnnounceWave(string waveText)
    {
        if (_waveAnnounceText != null)
        {
            _waveAnnounceText.text = waveText;
        }
    }

    public void UpdateAbilityDisplay(string abilityName, int abilityCound)
    {
        if (_abilityDisplayText != null)
        {
            _abilityDisplayText.text = $"{abilityCound} X {abilityName}";
        }
    }

    public void ClearAbilityDisplay()
    {
        if (_abilityDisplayText != null)
        {
            _abilityDisplayText.text = "No Ability";
        }
        DisplayAbilityHelper(0);
    }

    void PlayBackgroundMusic()
    {
        if (_gameBackgroundMusic != null && GameSettings.Instance != null && GameSettings.Instance.IsMusicEnabled)
        {
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.clip = _gameBackgroundMusic;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }

    void UpdateAbilityTimers()
    {
        if (_powerUpSystem != null)
        {
            //POWER BOOST
            if (_powerBoostTimerImage != null)
            {
                if (_powerUpSystem.HasPowerBoost)
                {
                    _powerBoostTimerImage.gameObject.SetActive(true);
                    float fillAmount = _powerUpSystem.PowerBoostRemainingTime / _powerUpSystem.PowerBoostDuration;
                    _powerBoostTimerImage.fillAmount = fillAmount;
                }
                else
                {
                    _powerBoostTimerImage.gameObject.SetActive(false);
                }
            }

            //POWER JUMP
            if (_powerJumpTimerImage != null)
            {
                if (_powerUpSystem.PowerJumpPowerUpActive)
                {
                    _powerJumpTimerImage.gameObject.SetActive(true);
                    float fillAmount = _powerUpSystem.PowerJumpRemainingTime / _powerUpSystem.PowerJumpDuration;
                    _powerJumpTimerImage.fillAmount = fillAmount;
                }
                else
                {
                    _powerJumpTimerImage.gameObject.SetActive(false);
                }
            }

            //INVISIBILITY
            if (_InvisibilityTimerImage != null)
            {
                if (_powerUpSystem.InvisibilityPowerUpActive)
                {
                    _InvisibilityTimerImage.gameObject.SetActive(true);
                    float fillAmount = _powerUpSystem.InvisibilityRemainingTime / _powerUpSystem.InvisibilityPowerUpDuration;
                    _InvisibilityTimerImage.fillAmount = fillAmount;
                }
                else
                {
                    _InvisibilityTimerImage.gameObject.SetActive(false);
                }
            }
        }
    }

    public void DisplayAbilityHelper(int abilityAmount)
    {
        if (_useAbilityText != null && _prevAbilityText != null && _nextAbilityText != null)
        {
            switch (abilityAmount)
            {
                case 0:
                    _useAbilityText.gameObject.SetActive(false);
                    _nextAbilityText.gameObject.SetActive(false);
                    _prevAbilityText.gameObject.SetActive(false);
                    break;
                case 1:
                    _useAbilityText.gameObject.SetActive(true);
                    _nextAbilityText.gameObject.SetActive(false);
                    _prevAbilityText.gameObject.SetActive(false);
                    break;
                default:
                    _useAbilityText.gameObject.SetActive(true);
                    _nextAbilityText.gameObject.SetActive(true);
                    _prevAbilityText.gameObject.SetActive(true);
                    break;
            }
        }
        else
        {
            Debug.LogError("Ability helpre text elements are not assigned!");
        }
    }
}
