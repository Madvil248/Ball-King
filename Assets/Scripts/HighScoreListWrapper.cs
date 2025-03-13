using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HighScoreListWrapper
{
    public List<HighScoreEntry> highScoreList;

    public HighScoreListWrapper()
    {
        highScoreList = new List<HighScoreEntry>();
    }
}
