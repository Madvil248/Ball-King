using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _gameTitleText;
    [SerializeField] private RectTransform _gameTitleRectTransform;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _highScoresButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _quitGameButton;

    [Header("High Scores Panel")]
    [SerializeField] private GameObject _highScorePanel;
    [SerializeField] private TextMeshProUGUI _highScoreListText;
    [SerializeField] private Button _backToMainMenuButton;
    [SerializeField] private TextMeshProUGUI _highScoresTitleText;

    [Header("Options Panel")]
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private Button _backFromOptionsButton;

    [Header("Background Music")]
    [SerializeField] private AudioClip _backgroundMusic;
    [SerializeField] private AudioClip _buttonClickSound;
    private AudioSource _audioSource;

    private Vector2 _originalGameTitlePosition;
    private bool _originalPositionSaved = false;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        UpdateMusicState();

        if (_optionsPanel != null)
        {
            _optionsPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Options Panel not assigned in MainMenuUIController!");
        }
    }

    void Update()
    {
        UpdateMusicState();
    }

    public void OnPlayButtonClicked()
    {
        PlayClickOnButtonClick();
        SceneManager.LoadScene("GameMain");
    }

    public void OnHighScoresButtonClicked()
    {
        PlayClickOnButtonClick();
        if (!_originalPositionSaved)
        {
            _originalGameTitlePosition = _gameTitleRectTransform.anchoredPosition;
            _originalPositionSaved = true;
        }
        _gameTitleRectTransform.anchoredPosition = new Vector2(_gameTitleRectTransform.anchoredPosition.x, -100);

        GameManager gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            List<HighScoreEntry> highScores = gameManager.GetHighScores();

            string highScoreString = FormatHighScores(highScores);
            _highScoreListText.text = highScoreString;

            _highScorePanel.gameObject.SetActive(true);
            _highScoresTitleText.gameObject.SetActive(true);
            SetMainMenuButtonsActive(false);
        }
        else
        {
            Debug.LogError("GameManager Instance not found! Cannot retrieve high scores.");
        }
    }

    public void OnOptionsButtonClicked()
    {
        PlayClickOnButtonClick();
        SetMainMenuButtonsActive(false);
        if (_optionsPanel != null)
        {
            _optionsPanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Options Panel not assigned in MainMenuUIController!");
        }
    }

    public void OnQuitGameButtonClicked()
    {
        PlayClickOnButtonClick();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void OnBackToMainMenuButtonClicked()
    {
        PlayClickOnButtonClick();
        _highScorePanel.gameObject.SetActive(false);
        SetMainMenuButtonsActive(true);

        _gameTitleRectTransform.anchoredPosition = _originalGameTitlePosition;
    }

    public void OnBackFromOptionsButtonClicked()
    {
        PlayClickOnButtonClick();
        if (_optionsPanel != null)
        {
            _optionsPanel.gameObject.SetActive(false);
        }
        SetMainMenuButtonsActive(true);
    }

    private string FormatHighScores(List<HighScoreEntry> scores)
    {
        string formattedText = "";
        if (scores != null && scores.Count > 0)
        {
            for (int i = 0; i < scores.Count; i++)
            {
                HighScoreEntry entry = scores[i];
                formattedText += $"{i + 1} {entry.PlayerName} {entry.Score}\n";
            }
        }
        else
        {
            formattedText = "No High Scores yet!";
        }
        return formattedText;
    }

    private void SetMainMenuButtonsActive(bool active)
    {
        _playButton.gameObject.SetActive(active);
        _highScoresButton.gameObject.SetActive(active);
        _optionsButton.gameObject.SetActive(active);
        _quitGameButton.gameObject.SetActive(active);
    }

    private void PlayClickOnButtonClick()
    {
        if (_audioSource != null && _buttonClickSound != null && GameSettings.Instance != null && GameSettings.Instance.IsSoundEnabled)
        {
            _audioSource.PlayOneShot(_buttonClickSound);
        }
    }

    private void UpdateMusicState()
    {
        if (_backgroundMusic != null && GameSettings.Instance != null && GameSettings.Instance.IsMusicEnabled)
        {
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = _backgroundMusic;
                _audioSource.loop = true;
                _audioSource.Play();
            }
        }
        else
        {
            if (_audioSource != null && _audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
        }
    }
}
