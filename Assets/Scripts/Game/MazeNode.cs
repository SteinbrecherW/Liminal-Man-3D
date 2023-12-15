using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeNode
{
    public bool OpenUp = false;
    public bool OpenRight = false;
    public bool OpenDown = false;
    public bool OpenLeft = false;
    public bool OpenAll
    {
        get
        {
            return OpenUp && OpenRight && OpenDown && OpenLeft;
        }
    }

    public bool Visited = false;
}

