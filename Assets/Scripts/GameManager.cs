using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private List<HighScoreEntry> _highScoreList = new List<HighScoreEntry>();

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
    }

    void Start()
    {
        Debug.Log("GameManager Start() called.");
        AddHighScore("Alice", 1500);
        AddHighScore("Bob", 2200);
        AddHighScore("Charlie", 1800);
        AddHighScore("David", 900);
        AddHighScore("Eve", 2500);
        AddHighScore("Frank", 1200);
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
        PrintHighScoresToConsole();
    }

    public List<HighScoreEntry> GetHighScores()
    {
        return _highScoreList;
    }

    private void PrintHighScoresToConsole()
    {
        Debug.Log("--- High Score List (Count: " + _highScoreList.Count + ") ---");
        foreach (HighScoreEntry entry in _highScoreList)
        {
            Debug.Log($"{entry.PlayerName}: {entry.Score}");
        }
        Debug.Log("--- End High Score List ---");
    }
}
