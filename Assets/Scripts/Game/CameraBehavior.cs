using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
[AddComponentMenu("Camera-Control/Smooth Mouse Look")]
public class SmoothMouseLook : MonoBehaviour
{
    // horizontal rotation speed
    [SerializeField] float _horizontalSpeed = 1f;
    // vertical rotation speed
    [SerializeField] float _verticalSpeed = 1f;
    float _xRotation = 0.0f;
    float _yRotation = 0.0f;
    [SerializeField] Camera _camera;

    [SerializeField] float _fadeInTime = 3.0f;
    [SerializeField] float _fadeOutTime = 6.0f;

    [SerializeField] Image _fadeInImage;

    [SerializeField] TextMeshProUGUI _winText;

    bool _setup = true;
    bool _fadeout = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _winText.enabled = false;
        StartCoroutine(FadeInImage());
    }

    void Update()
    {
        if (_setup && MazeBehavior.Instance.MazeInitialized)
        {
            MazeNode mn = MazeBehavior.Instance.Map[MazeBehavior.Instance.MapSizeX / 2, MazeBehavior.Instance.MapSizeZ / 2];
            if (mn.OpenLeft)
                _yRotation = -90;
            else if (mn.OpenRight)
                _yRotation = 90;
            else if (mn.OpenDown)
                _yRotation = 180;

            _camera.transform.eulerAngles = new Vector3(_xRotation, _yRotation, 0.0f);

            _setup = false;
        }
        if (GameBehavior.Instance.CurrentState == GameState.Running)
        {
            float mouseX = Input.GetAxis("Mouse X") * _horizontalSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * _verticalSpeed;

            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90, 90);

            _camera.transform.eulerAngles = new Vector3(_xRotation, _yRotation, 0.0f);
        }
        else if(GameBehavior.Instance.CurrentState == GameState.Lose && !_fadeout)
        {
            _fadeout = true;
            StartCoroutine(FadeOutImage());
        }
        else if (GameBehavior.Instance.CurrentState == GameState.Win && !_fadeout)
        {
            _fadeout = true;
            _winText.enabled = true;
            StartCoroutine(FadeOutImage());
        }
    }

    IEnumerator FadeInImage()
    {
        _fadeInImage.CrossFadeAlpha(0, _fadeInTime, false);
        yield return new WaitForSeconds(_fadeInTime);
        _fadeInImage.enabled = false;
        GameBehavior.Instance.CurrentState = GameState.Running;
    }

    IEnumerator FadeOutImage()
    {
        _fadeInImage.enabled = true;
        _fadeInImage.CrossFadeAlpha(1, _fadeInTime, false);
        yield return new WaitForSeconds(_fadeOutTime);
        if(GameBehavior.Instance.CurrentState == GameState.Lose)
            GameBehavior.Instance.LoseScene();
        else
            GameBehavior.Instance.WinScene();
    }
}
