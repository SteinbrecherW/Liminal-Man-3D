using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithCamera : MonoBehaviour
{
    [SerializeField] Transform _camera;

    void Update()
    {
        transform.rotation = _camera.rotation;
    }
}
