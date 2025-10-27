using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public struct ScoreEntry
{
    public string playerName;
    public int score;
}

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    public int maxEntries = 5;
    private ScoreEntry[] scores;

    string filePath;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "highscores.json");
        Load();
    }

    public ScoreEntry[] GetTopScores()
    {
        return scores;
    }

    public void AddScore(string name, int score)
    {
        if (string.IsNullOrEmpty(name)) name = "Player";

        // 새 점수 넣을 자리를 찾는다
        for (int i = 0; i < maxEntries; i++)
        {
            if (score > scores[i].score)
            {
                // 밀기
                for (int j = maxEntries - 1; j > i; j--)
                {
                    scores[j] = scores[j - 1];
                }
                // 삽입
                scores[i] = new ScoreEntry { playerName = name, score = score };
                break;
            }
        }
        Save();
    }

    void Save()
    {
        string json = JsonUtility.ToJson(new Wrapper { scores = this.scores }, true);
        File.WriteAllText(filePath, json);
    }

    void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            var wrapper = JsonUtility.FromJson<Wrapper>(json);
            this.scores = wrapper.scores;
        }
        else
        {
            this.scores = new ScoreEntry[maxEntries];
        }
    }

    [Serializable]
    class Wrapper
    {
        public ScoreEntry[] scores;
    }
}