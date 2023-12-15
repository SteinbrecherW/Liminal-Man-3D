using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorData : MonoBehaviour
{
    public bool OpenUp = false;
    public bool OpenRight = false;
    public bool OpenDown = false;
    public bool OpenLeft = false;

    public bool Compare(MazeNode node)
    {
        Debug.Log("Comparing. Values:\n" +
            OpenUp + ", " + OpenLeft + ", " + OpenDown + ", " + OpenLeft + "\n" +
            node.OpenUp + ", " + node.OpenRight + ", " + node.OpenDown + ", " + node.OpenLeft + ", ");

        if (OpenUp == node.OpenUp &&
            OpenLeft == node.OpenLeft &&
            OpenRight == node.OpenRight &&
            OpenDown == node.OpenDown)
            return true;
        return false;
    }
}
