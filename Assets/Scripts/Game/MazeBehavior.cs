using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBehavior : MonoBehaviour
{
    public static MazeBehavior Instance;

    public int MapSizeX = 12;
    public int MapSizeZ = 12;

    public MazeNode[,] Map;

    [SerializeField] CorridorData[] _corridors;

    [HideInInspector] public bool MazeInitialized = false;

    [SerializeField] int _extraOpenings = 5;
    List<bool> _breakWall;

    enum Direction
    {
        Empty,
        North,
        East,
        South,
        West,
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        if (MapSizeX < 2)
            MapSizeX = 2;

        if (MapSizeZ < 2)
            MapSizeZ = 2;

        _breakWall = new List<bool>();
        for(int i = 0; i < (MapSizeX * MapSizeZ) - ((MapSizeX * MapSizeZ / 2) - 4); i++)
        {
            if (i < _extraOpenings)
                _breakWall.Add(true);
            else
                _breakWall.Add(false);
        }

        GenerateMaze();
    }

    void GenerateMaze()
    {
        Map = new MazeNode[MapSizeX, MapSizeZ];

        for (int z = 0; z < MapSizeZ; z++)
            for (int x = 0; x < MapSizeX; x++)
                Map[x, z] = new MazeNode();
            
        CorridorSetup(0, 0);
        PlaceCorridors();
    }

    void CorridorSetup(int x, int z, Direction incomingDirection = Direction.Empty)
    {
        Debug.Log("Setting up corridor at x : " + x + ", z : " + z);
        Map[x, z].Visited = true;

        if(incomingDirection != Direction.Empty)
        {
            switch (incomingDirection)
            {
                case Direction.North:
                    Map[x, z].OpenDown = true;
                    break;

                case Direction.East:
                    Map[x, z].OpenLeft = true;
                    break;

                case Direction.South:
                    Map[x, z].OpenUp = true;
                    break;

                case Direction.West:
                    Map[x, z].OpenRight = true;
                    break;
            }
        }

        List<Direction> directions = new List<Direction>();
        directions.Add(Direction.North);
        directions.Add(Direction.East);
        directions.Add(Direction.South);
        directions.Add(Direction.West);
        directions.Shuffle();

        //Recursive backtracking maze algorithm
        foreach(Direction dir in directions)
        {
            switch (dir)
            {
                case Direction.North:
                    if (z + 1 < MapSizeZ && !Map[x, z + 1].Visited)
                    {
                        Map[x, z].OpenUp = true;
                        CorridorSetup(x, z + 1, dir);
                    }
                    break;

                case Direction.East:
                    if (x + 1 < MapSizeX && !Map[x + 1, z].Visited)
                    {
                        Map[x, z].OpenRight = true;
                        CorridorSetup(x + 1, z, dir);
                    }
                    break;

                case Direction.South:
                    if (z - 1 >= 0 && !Map[x, z - 1].Visited)
                    {
                        Map[x, z].OpenDown = true;
                        CorridorSetup(x, z - 1, dir);
                    }
                    break;

                case Direction.West:
                    if (x - 1 >= 0 && !Map[x - 1, z].Visited)
                    {
                        Map[x, z].OpenLeft = true;
                        CorridorSetup(x - 1, z, dir);
                    } 
                    break;
            }
        }

        //Breaks extra walls to make the maze more open
        if (z != 0 && x != 0 && x + 1 < MapSizeX && z + 1 < MapSizeZ)
        {
            bool test = _breakWall[Random.Range(0, _breakWall.Count)];
            if (test)
            {
                Debug.Log("Breaking wall at x:" + x + ", z:" + z);
                bool breaking = true;
                int index = Random.Range(0, 4);
                if(Map[x, z].OpenAll)
                {
                    Debug.Log("Intersection found, no break made");
                    breaking = false;
                }
                while (breaking)
                {
                    switch (index)
                    {
                        case 0:
                            if (Map[x, z].OpenUp)
                                index++;
                            else
                            {
                                Map[x, z].OpenUp = true;
                                Map[x, z + 1].OpenDown = true;
                                breaking = false;
                            }
                            break;

                        case 1:
                            if (Map[x, z].OpenLeft)
                                index++;
                            else
                            {
                                Map[x, z].OpenLeft = true;
                                Map[x - 1, z].OpenRight = true;
                                breaking = false;
                            }
                            break;

                        case 2:
                            if (Map[x, z].OpenRight)
                                index++;
                            else
                            {
                                Map[x, z].OpenRight = true;
                                Map[x + 1, z].OpenLeft = true;
                                breaking = false;
                            }
                            break;

                        case 3:
                            if (Map[x, z].OpenDown)
                                index = 0;
                            else
                            {
                                Map[x, z].OpenDown = true;
                                Map[x, z - 1].OpenUp = true;
                                breaking = false;
                            }
                            break;
                    }
                }
            }
            _breakWall.Remove(test);
        }

        Debug.Log("Finished corridor at x : " + x + ", z : " + z);
    }

    void PlaceCorridors()
    {
        for(int z = 0; z < MapSizeZ; z++)
        {
            for(int x = 0; x < MapSizeX; x++)
            {
                bool unpopulated = true;
                int i = 0;
                while (unpopulated)
                {
                    if (i >= _corridors.Length)
                    {
                        Debug.Log("Couldn't find matching piece");
                        Instantiate(_corridors[15], new Vector3(x * 8, 0, z * 8), Quaternion.identity, transform);
                        unpopulated = false;
                    }
                    else if (_corridors[i].Compare(Map[x, z]))
                    {
                        Instantiate(_corridors[i], new Vector3(x * 8, 0, z * 8), Quaternion.identity, transform);
                        unpopulated = false;
                    }
                    else
                        i++;
                }
                Debug.Log("Map populated!");
            }
        }
        MazeInitialized = true;
    }
}
