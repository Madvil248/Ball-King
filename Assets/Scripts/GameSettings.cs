using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty CurrentDifficulty { get; private set; }
    public bool IsSoundEnabled { get; private set; }
    public bool IsMusicEnabled { get; private set; }
    public bool UseCameraRelativeMovement { get; private set; }

    private const string DifficultyKey = "Difficulty";
    private const string SoundKey = "SoundEnabled";
    private const string MusicKey = "MusicEnabled";
    private const string CameraRelativeKey = "CameraRelative";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        CurrentDifficulty = difficulty;
        PlayerPrefs.SetInt(DifficultyKey, (int)difficulty);
        PlayerPrefs.Save();
        Debug.Log($"Difficulty set to: {CurrentDifficulty}");
    }

    public void SetSoundEnabled(bool enabled)
    {
        IsSoundEnabled = enabled;
        PlayerPrefs.SetInt(SoundKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"Sound enabled: {IsSoundEnabled}");
    }

    public void SetMusicEnabled(bool enabled)
    {
        IsMusicEnabled = enabled;
        PlayerPrefs.SetInt(MusicKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"Music enabled: {IsMusicEnabled}");
    }

    public void SetUseCameraRelativeMovement(bool useRelative)
    {
        UseCameraRelativeMovement = useRelative;
        PlayerPrefs.SetInt(CameraRelativeKey, useRelative ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"Use Camera Relative Movement: {UseCameraRelativeMovement}");
    }

    public void LoadSettings()
    {
        CurrentDifficulty = (Difficulty)PlayerPrefs.GetInt(DifficultyKey, 1);
        IsSoundEnabled = PlayerPrefs.GetInt(SoundKey, 1) == 1;
        IsMusicEnabled = PlayerPrefs.GetInt(MusicKey, 1) == 1;
        UseCameraRelativeMovement = PlayerPrefs.GetInt(CameraRelativeKey, 1) == 0;

        Debug.Log($"Loaded Settings - Difficulty: {CurrentDifficulty}, Sound: {IsSoundEnabled}, Music: {IsMusicEnabled}, Camera Relative: {UseCameraRelativeMovement}");
    }
}
