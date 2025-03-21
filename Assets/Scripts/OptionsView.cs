using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsView : MonoBehaviour
{
    [Header("Difficulty")]
    [SerializeField] private TMP_Dropdown _difficultyDropdown;
    private GameSettings.Difficulty _currentSelectedDifficulty;

    [Header("Audio")]
    [SerializeField] private Toggle _soundToggle;
    [SerializeField] private Toggle _musicToggle;

    [Header("Camera")]
    [SerializeField] private Toggle _cameraToggle;

    void Start()
    {
        if (GameSettings.Instance != null)
        {
            if (_difficultyDropdown != null)
            {
                _difficultyDropdown.ClearOptions();
                foreach (var enumValue in System.Enum.GetValues(typeof(GameSettings.Difficulty)))
                {
                    _difficultyDropdown.options.Add(new TMP_Dropdown.OptionData(enumValue.ToString()));
                }
                Debug.Log($"CurrentDifficulty - {GameSettings.Instance.CurrentDifficulty}");
                _difficultyDropdown.value = (int)GameSettings.Instance.CurrentDifficulty;
                _difficultyDropdown.RefreshShownValue();
                _currentSelectedDifficulty = GameSettings.Instance.CurrentDifficulty;
            }

            if (_soundToggle != null)
            {
                Debug.Log($"IsSoundEnabled - {GameSettings.Instance.IsSoundEnabled}");
                _soundToggle.isOn = GameSettings.Instance.IsSoundEnabled;
            }

            if (_musicToggle != null)
            {
                _musicToggle.isOn = GameSettings.Instance.IsMusicEnabled;
            }

            if (_cameraToggle != null)
            {
                _cameraToggle.isOn = GameSettings.Instance.UseCameraRelativeMovement;
            }
        }
        else
        {
            Debug.LogError("GameSettings Instance not found in the scene!");
        }
    }

    public void OnDifficultyChange(int index)
    {
        if (GameSettings.Instance != null)
        {
            Debug.Log($"OnDifficultyChange index - {index}");
            _currentSelectedDifficulty = (GameSettings.Difficulty)index;
            Debug.Log($"_currentSelectedDifficulty - {_currentSelectedDifficulty}");
            GameSettings.Instance.SetDifficulty(_currentSelectedDifficulty);
        }
    }

    public void OnSoundToggle(bool isOn)
    {

        if (GameSettings.Instance != null)
        {
            Debug.Log($"OnSoundToggle | isOn - {isOn}");
            GameSettings.Instance.SetSoundEnabled(isOn);
        }
    }

    public void OnMusicToggle(bool isOn)
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetMusicEnabled(isOn);
        }
    }

    public void OnCameraToggle(bool isOn)
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetUseCameraRelativeMovement(isOn);
        }
    }
}
