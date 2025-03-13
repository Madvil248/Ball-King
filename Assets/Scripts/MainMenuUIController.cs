using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    private Vector2 _originalGameTitlePosition;
    private bool _originalPositionSaved = false;

    void Start()
    {

    }

    void Update()
    {

    }

    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("GameMain");
        Debug.Log("Play Button Clicked - Loading Game Scene!");
    }

    public void OnHighScoresButtonClicked()
    {
        Debug.Log("High Scores Button Clicked - (Functionality to be implemented later)");

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
        Debug.Log("Options Button Clicked - (Functionality to be implemented later)");
    }

    public void OnQuitGameButtonClicked()
    {
        Debug.Log("Quit Game Button Clicked - Quitting Application");
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void OnBackToMainMenuButtonClicked()
    {
        Debug.Log("Back to Main Menu Button Clicked - Hiding High Scores Panel.");
        _highScorePanel.gameObject.SetActive(false);
        SetMainMenuButtonsActive(true);

        _gameTitleRectTransform.anchoredPosition = _originalGameTitlePosition;
    }

    private string FormatHighScores(List<HighScoreEntry> scores)
    {
        string formattedText = "";
        if (scores != null && scores.Count > 0)
        {
            for (int i = 0; i < scores.Count; i++)
            {
                HighScoreEntry entry = scores[i];
                formattedText += $"{i + 1} {entry.PlayerName} - {entry.Score}\n";
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
}
