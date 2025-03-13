using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private TextMeshProUGUI _finalScoreText;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private Button _submitScoreButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    void Start()
    {
        int finalScore = GameOverData.FinalScore;
        SetFinalScore(finalScore);
        Debug.Log("Game Over Scene Loaded. Final Score: " + finalScore);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetFinalScore(int score)
    {
        _finalScoreText.text = "Your Score " + score.ToString();
    }

    public void OnSubmitScoreButtonClicked()
    {
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
            PrintHighScoresToConsole(gameManager);
        }
        else
        {
            Debug.LogError("GameManager Instance not found! Cannot submit high score.");
        }
        _nameInputField.text = "";
    }

    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene("GameMain");
        Debug.Log("Restart Button Clicked!");
    }

    public void OnMainMenuButtonClicked()
    {
        //SceneManager.LoadScene("MainMenuScene");
        Debug.Log("Main Menu Button Clicked!");
    }

    private void PrintHighScoresToConsole(GameManager gm)
    {
        Debug.Log("--- High Score List from GameManager (Count: " + gm.GetHighScores().Count + ") ---");

        foreach (HighScoreEntry entry in gm.GetHighScores())
        {
            Debug.Log($"{entry.PlayerName}: {entry.Score}");
        }
        Debug.Log("--- End High Score List from GameManager ---");
    }
}
