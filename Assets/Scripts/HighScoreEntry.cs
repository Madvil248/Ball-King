using System;

[System.Serializable]
public class HighScoreEntry
{
    public string PlayerName;
    public int Score;

    public HighScoreEntry(string playerName, int score)
    {
        PlayerName = playerName;
        Score = score;
    }
}
