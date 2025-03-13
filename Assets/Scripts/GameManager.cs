using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private List<HighScoreEntry> _highScoreList = new List<HighScoreEntry>();
    private const string HighScoreKey = "HighScoreList";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager initialized and set to DontDestroyOnLoad.");
        }
        else
        {
            Debug.LogWarning("Another GameManager instance detected! Destroying the new one.");
            Destroy(gameObject);
        }
        LoadHighScores();
    }

    void Start()
    {
        Debug.Log("GameManager Start() called.");
    }

    void Update()
    {

    }

    public void AddHighScore(string playerName, int score)
    {
        HighScoreEntry newEntry = new HighScoreEntry(playerName, score);
        _highScoreList.Add(newEntry);
        _highScoreList.Sort((a, b) => b.Score.CompareTo(a.Score));
        Debug.Log($"High Score Added: {playerName} - {score}. High Score List Count: {_highScoreList.Count}");
        SaveHighScores();
    }

    public Tuple<List<HighScoreEntry>, int> GetTopHighScores(int count, string currentPlayerName, int currentPlayerScore)
    {
        _highScoreList.Sort((a, b) => b.Score.CompareTo(a.Score));
        List<HighScoreEntry> topScores = _highScoreList.GetRange(0, Mathf.Min(count, _highScoreList.Count));
        int playerRank = -1;

        bool scoreInTop = false;
        foreach (var scoreEntry in topScores)
        {
            if (scoreEntry.PlayerName == currentPlayerName && scoreEntry.Score == currentPlayerScore)
            {
                scoreInTop = true;
                break;
            }
        }

        if (!scoreInTop)
        {
            for (int i = 10; i < _highScoreList.Count; i++)
            {
                if (_highScoreList[i].PlayerName == currentPlayerName && _highScoreList[i].Score == currentPlayerScore)
                {
                    playerRank = i + 1;
                    break;
                }
            }

            if (playerRank != -1)
            {
                List<HighScoreEntry> extendedList = new List<HighScoreEntry>(topScores);
                extendedList.Add(new HighScoreEntry(currentPlayerName, currentPlayerScore));
                return Tuple.Create(extendedList, playerRank);
            }
        }

        return Tuple.Create(topScores, -1);
    }

    public List<HighScoreEntry> GetHighScores()
    {
        return _highScoreList;
    }

    public void SaveHighScores()
    {
        HighScoreListWrapper wrapper = new HighScoreListWrapper();
        wrapper.highScoreList = _highScoreList;

        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(HighScoreKey, json);
        PlayerPrefs.Save();
        Debug.Log("High Scores Saved to PlayerPrefs");
    }

    public void LoadHighScores()
    {
        string json = PlayerPrefs.GetString(HighScoreKey, "");

        if (!string.IsNullOrEmpty(json))
        {
            HighScoreListWrapper wrapper = JsonUtility.FromJson<HighScoreListWrapper>(json);
            if (wrapper != null && wrapper.highScoreList != null)
            {
                _highScoreList = wrapper.highScoreList;
                Debug.Log($"High Scores Loaded from PlayerPrefs. Count: {_highScoreList.Count}");
            }
            else
            {
                Debug.LogWarning("Failed to deserialize High Score List from PlayerPrefs. Starting with an empty list.");
                _highScoreList = new List<HighScoreEntry>();
            }
        }
        else
        {
            Debug.Log("No High Scores found in PlayerPrefs. Starting with an empty list.");
            _highScoreList = new List<HighScoreEntry>();
        }
    }

    private void OnApplicationQuit()
    {
        SaveHighScores();
        Debug.Log("High Scores Saved on Application Quit");
    }
}

