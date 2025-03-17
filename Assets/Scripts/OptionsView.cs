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
                _difficultyDropdown.value = (int)GameSettings.Instance.CurrentDifficulty;
                _currentSelectedDifficulty = GameSettings.Instance.CurrentDifficulty;
            }

            if (_soundToggle != null)
            {
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
            _currentSelectedDifficulty = (GameSettings.Difficulty)index;
        }
    }

    public void OnSoundToggle(bool isOn)
    {
        if (GameSettings.Instance != null)
        {
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
