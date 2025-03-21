using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private TextMeshProUGUI _finalScoreText;
    [SerializeField] private TextMeshProUGUI _topHighScoresText;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private Button _submitScoreButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip _buttonClick;
    [SerializeField] private AudioClip _backgroundMusic;
    [SerializeField] private float _backgroundMusicStartTime = 60f;
    private AudioSource _audioSource;

    private int _finalScore;

    void Start()
    {
        _finalScore = GameOverData.FinalScore;
        SetFinalScore(_finalScore);
        Debug.Log("Game Over Scene Loaded. Final Score: " + _finalScore);

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        PlayBackgroundMusic();
    }

    void Update()
    {

    }

    public void SetFinalScore(int score)
    {
        _finalScoreText.text = "Your Score " + score.ToString();
    }

    public void OnSubmitScoreButtonClicked()
    {
        PlaySoundButtonClick();
        string playerName = _nameInputField.text;
        Debug.Log("Score Submitted! Player Name: " + playerName + ", Score: " + /* we'll get the score later */ "...");

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Player name cannot be empty. Please enter a name.");
            return;
        }

        int finalScore = GameOverData.FinalScore;
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            gameManager.AddHighScore(playerName, finalScore);
            Debug.Log($"Score Submitted to GameManager! Player: {playerName}, Score: {finalScore}");
        }
        else
        {
            Debug.LogError("GameManager Instance not found! Cannot submit high score.");
        }
        _nameInputField.text = "";
        _finalScoreText.gameObject.SetActive(false);
        _nameInputField.gameObject.SetActive(false);
        _submitScoreButton.gameObject.SetActive(false);
        DisplayTopHighScores(playerName, _finalScore);
    }

    public void OnRestartButtonClicked()
    {
        PlaySoundButtonClick();
        SceneManager.LoadScene("GameMain");
        Debug.Log("Restart Button Clicked!");
    }

    public void OnMainMenuButtonClicked()
    {
        PlaySoundButtonClick();
        SceneManager.LoadScene("MainMenuScene");
        Debug.Log("Main Menu Button Clicked!");
    }

    private void DisplayTopHighScores(string playerName, int playerScore)
    {
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            Tuple<List<HighScoreEntry>, int> highScoreData = gameManager.GetTopHighScores(10, playerName, playerScore);
            List<HighScoreEntry> topScores = highScoreData.Item1;
            int playerRank = highScoreData.Item2;

            string highScoresText = "";
            for (int i = 0; i < Mathf.Min(10, topScores.Count); i++)
            {
                highScoresText += $"{i + 1} {topScores[i].PlayerName} - {topScores[i].Score}\n";
            }
            if (playerRank > 10)
            {
                highScoresText += $"(Your Score)\n";
                highScoresText += $"{playerRank} {playerName} - {playerScore}\n";
            }
            _topHighScoresText.text = highScoresText;
        }
        _topHighScoresText.gameObject.SetActive(true);
    }

    void PlayBackgroundMusic()
    {
        if (_backgroundMusic != null && GameSettings.Instance != null && GameSettings.Instance.IsMusicEnabled)
        {
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.clip = _backgroundMusic;
            _audioSource.time = _backgroundMusicStartTime;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }

    void PlaySoundButtonClick()
    {
        if (_buttonClick != null && GameSettings.Instance != null && GameSettings.Instance.IsSoundEnabled)
        {
            if (_audioSource == null)
            {
                _audioSource = gameObject.GetComponent<AudioSource>();
            }
            _audioSource.PlayOneShot(_buttonClick);
        }
    }
}
