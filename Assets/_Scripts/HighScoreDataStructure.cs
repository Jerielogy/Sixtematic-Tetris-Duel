using System;
using System.Collections.Generic;


[Serializable]
public class HighScoreEntry
{
    public int score;
    public string name;
    public string team;
}

[Serializable]
public class HighScoreData
{
    public List<HighScoreEntry> scores = new List<HighScoreEntry>();
}