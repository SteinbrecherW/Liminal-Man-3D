using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapBehavior : MonoBehaviour
{
    void Start()
    {
        transform.position = new Vector3((MazeBehavior.Instance.MapSizeX * 8 / 2) - 4, (MazeBehavior.Instance.MapSizeX * 15 / 2) + 5, (MazeBehavior.Instance.MapSizeZ * 8 / 2) - 4);
    }
}
