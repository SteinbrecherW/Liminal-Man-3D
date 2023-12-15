using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInImageBehavior : MonoBehaviour
{
    RawImage _image;

    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<RawImage>();
        _image.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameBehavior.Instance.CurrentState == GameState.Running)
            _image.enabled = true;
        else
            _image.enabled = false;
    }
}
