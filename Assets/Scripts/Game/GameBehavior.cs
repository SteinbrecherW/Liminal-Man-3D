using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameBehavior : MonoBehaviour
{
    public static GameBehavior Instance;

    public GameState CurrentState;

    Scene _scene;

    [SerializeField] GameObject _enemy;

    public List<EnemyBehavior> Enemies;

    bool _enemySpawning = true;

    [SerializeField] AudioMixer _mix;
    float _mixTimer = 6;

    [SerializeField] GameObject _pauseMenu;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _scene = SceneManager.GetActiveScene();
    }

    void Start()
    {
        CurrentState = GameState.FadeIn;

        Enemies = new List<EnemyBehavior>();

        _mix.SetFloat("FXVolume", 3);

        _pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (_enemySpawning && MazeBehavior.Instance.MazeInitialized)
        {
            SpawnEnemy();
            _enemySpawning = false;
        }

        if(CurrentState == GameState.Running && Input.GetKeyDown(KeyCode.Escape))
        {
            _pauseMenu.SetActive(true);
            CurrentState = GameState.Paused;
        }

        else if(CurrentState == GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
        {
            _pauseMenu.SetActive(false);
            CurrentState = GameState.Running;
        }

        else if (CurrentState == GameState.Paused && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("StartMenu");
        }

        else if(CurrentState == GameState.Lose)
        {
            _mixTimer -= Time.deltaTime;
            _mix.SetFloat("FXVolume", Mathf.Lerp(-48, 3, _mixTimer / 6));
        }
    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(_enemy, new Vector3(0, -4, 0), Quaternion.identity, transform);
        Enemies.Add(enemy.GetComponent<EnemyBehavior>());

        GameObject enemy2 = Instantiate(_enemy, new Vector3((MazeBehavior.Instance.MapSizeX - 1) * 8, -4, (MazeBehavior.Instance.MapSizeZ - 1) * 8), Quaternion.identity, transform);
        EnemyBehavior eB = enemy2.GetComponent<EnemyBehavior>();
        eB.GridLocationX = 7;
        eB.GridLocationZ = 7;
        eB.EnemyIndex = 1;
        Enemies.Add(eB);
    }

    public void WinGame()
    {
        CurrentState = GameState.Win;
        Debug.Log("You won!");

        SceneManager.LoadScene(_scene.name);
    }

    public void LoseGame()
    {
        CurrentState = GameState.Lose;

        Debug.Log("You Lose!");
    }

    public void LoseScene()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void WinScene()
    {
        SceneManager.LoadScene(_scene.name);
    }
}

public enum GameState
{
    FadeIn,
    Running,
    Paused,
    Lose,
    Win
}
