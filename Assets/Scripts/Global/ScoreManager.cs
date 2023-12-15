using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    int _score = 1;
    public string PlayerName;
    List<int> _highScores = new List<int>();
    List<string> _playerNames = new List<string>();
    bool _currentHighScore = false;

    [SerializeField] TextMeshProUGUI _scoreText;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        _score = 0;
        PlayerName = "";

        Cursor.lockState = CursorLockMode.None;

        Load();
    }

    void Update()
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Leaderboard"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene("LiminalMan3D");

            else if (Input.GetKeyDown(KeyCode.Escape))
                SceneManager.LoadScene("StartMenu");
        }
    }

    public void AddScore(int points)
    {
        _score += points;
    }

    public void ResetScore()
    {
        _score = 0;
        PlayerName = "";
    }

    public void Save()
    {
        if (Directory.Exists(Application.dataPath + "/Save data/") == false)
            Directory.CreateDirectory(Application.dataPath + "/Save data/");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/Save data/Score.secure");
        ScoreData data = new ScoreData();

        int index = 9;
        while(data.highScores.Count < 10)
        {
            data.highScores.Insert(index, 0);
            data.playerNames.Insert(index, "???");
            index--;
        }

        bool scoreLogging = true;
        int i = 9;
        while (scoreLogging)
        {
            if (i < 0)
                scoreLogging = false;
            else if (i == 0 && _score > data.highScores[0])
            {
                data.highScores.Insert(i, _score);
                data.highScores.RemoveAt(10);

                data.playerNames.Insert(i, PlayerName);
                data.playerNames.RemoveAt(10);

                scoreLogging = false;
            }
            else if (_score > data.highScores[i] && _score <= data.highScores[i - 1])
            {
                data.highScores.Insert(i, _score);
                data.highScores.RemoveAt(10);

                data.playerNames.Insert(i, PlayerName);
                data.playerNames.RemoveAt(10);

                scoreLogging = false;
            }
            i--;
        }

        _highScores = data.highScores;
        _playerNames = data.playerNames;

        PrintScores();

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.dataPath + "/Save data/Score.secure"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/Save data/Score.secure", FileMode.Open);
            ScoreData data = (ScoreData)bf.Deserialize(file);
            file.Close();

            _highScores = data.highScores;
            _playerNames = data.playerNames;
        }
    }

    public void PrintScores()
    {
        string leaderboard = "";
        for(int i = 0; i < 10; i++)
        {
            leaderboard += $"{i}: {_playerNames[i]}, {_highScores[i]}\n";
        }
        leaderboard += "\nSpace to continue\nEscape to menu";
        _scoreText.enabled = true;
        _scoreText.text = leaderboard;
        ResetScore();
    }
}

[Serializable]
class ScoreData
{
    public List<int> highScores = new List<int>();
    public List<string> playerNames = new List<string>();
}
