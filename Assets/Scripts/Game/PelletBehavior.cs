using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PelletBehavior : MonoBehaviour
{
    public static PelletBehavior Instance;

    [SerializeField] GameObject _pellet;

    [SerializeField] TextMeshProUGUI _pelletText;

    public int Score = 0;
    int _pelletCount = 0;

    AudioSource _as;
    [SerializeField] AudioClip _pickupClip;

    public List<GameObject> Pellets = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        _pelletText.text = "00/XX";

        _as = GetComponent<AudioSource>();

        PopulateMap();
    }

    void Update()
    {
        if (GameBehavior.Instance.CurrentState == GameState.Running)
            _pelletText.enabled = true;
        else
            _pelletText.enabled = false;
    }

    void PopulateMap()
    {
        for(int z = 0; z < MazeBehavior.Instance.MapSizeZ; z++)
        {
            for(int x = 0; x < MazeBehavior.Instance.MapSizeX; x++)
            {
                if(!(x == MazeBehavior.Instance.MapSizeX / 2 && z == MazeBehavior.Instance.MapSizeZ / 2))
                {
                    GameObject currentPellet = Instantiate(_pellet, new Vector3(x * 8, -4, z * 8), Quaternion.identity, transform);
                    Pellets.Add(currentPellet);
                }
            }
        }

        _pelletCount = Pellets.Count;
        _pelletText.text = "00/" + _pelletCount;
    }

    public void RemovePellet(GameObject pellet)
    {
        Debug.Log("Removing pellet...");

        Pellets.Remove(pellet);

        Score++;
        if(Score < 10)
            _pelletText.text = "0" + Score + "/" + _pelletCount;
        else
            _pelletText.text = Score + "/" + _pelletCount;

        //ScoreManager.Instance.AddScore(1);

        if (Pellets.Count <= 0)
            GameBehavior.Instance.WinGame();

        //_as.PlayOneShot(_pickupClip);

        Destroy(pellet);
    }
}
