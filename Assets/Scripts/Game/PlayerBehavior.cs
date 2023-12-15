using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehavior : MonoBehaviour
{
    CharacterController _characterController;
    [SerializeField] float _movementSpeed = 1;
    [SerializeField] float _sprintModifier = 3;

    [SerializeField] float _gravity = 9.8f;
    [SerializeField] float _velocity = 0;

    [SerializeField] RectTransform _staminaBar;
    float _lerpValue = 0;

    [SerializeField] RawImage _bar;

    [SerializeField] Camera _camera;

    [SerializeField] FlashlightBehavior _flashlight;

    [SerializeField] float _maxSprintTimer = 2;
    float _sprintTimer;
    [SerializeField] float _maxExhaustedTimer = 4;
    float _exhausedTimer;

    [SerializeField] AudioSource _audio;

    Transform _enemy;

    bool _sprinting = false;
    public bool Sprinting
    {
        get => _sprinting;
        set
        {
            if (_sprinting != value)
            {
                _movementSpeed += value ? _sprintModifier : -_sprintModifier;
                _sprinting = value;
            }
        }
    }
    bool _exhausted = false;
    public bool Exhausted
    {
        get => _exhausted;
        set
        {
            if (_exhausted != value)
            {
                _movementSpeed -= value ? _sprintModifier : -_sprintModifier;
                _exhausted = value;
            }
        }
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        transform.SetPositionAndRotation(new Vector3(MazeBehavior.Instance.MapSizeX * 8 / 2, -4, MazeBehavior.Instance.MapSizeZ * 8 / 2), Quaternion.identity);

        _flashlight = GetComponentInChildren<FlashlightBehavior>();

        _sprintTimer = _maxSprintTimer;
        _exhausedTimer = _maxExhaustedTimer;

        _audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(GameBehavior.Instance.CurrentState == GameState.FadeIn)
            Fall();

        else if (GameBehavior.Instance.CurrentState == GameState.Running)
        {
            //If shift is pressed, start sprinting
            if (Input.GetKeyDown(KeyCode.LeftShift) && _sprintTimer >= 0 && !_exhausted)
                Sprinting = true;

            //System for handling sprint behavior
            //If player is exhausted from running out of stamina
            if (_exhausted)
            {
                _lerpValue = _exhausedTimer / _maxExhaustedTimer;
                _staminaBar.sizeDelta = new Vector2(Mathf.Lerp(3.2f, 0, _lerpValue), 100);

                //Timer will determine how long the player is exhausted for
                if (_exhausedTimer <= 0)
                {
                    Exhausted = false;
                    _exhausedTimer = _maxExhaustedTimer;
                    _bar.color = new Color32(57, 62, 229, 255);
                }
                else
                    _exhausedTimer -= Time.deltaTime;
            }
            //If the player isn't exhausted, but is sprinting
            else if (_sprinting)
            {
                _lerpValue = _sprintTimer / _maxSprintTimer;
                _staminaBar.sizeDelta = new Vector2(Mathf.Lerp(0, 3.2f, _lerpValue), 100);

                //Timer will determine how much stamina they have left
                //If they run out of stamina, they are exhausted
                if (_sprintTimer <= 0)
                {
                    Sprinting = false;
                    Exhausted = true;
                    _sprintTimer = _maxSprintTimer;
                    _bar.color = new Color32(200, 57, 88, 255);
                }
                else
                    _sprintTimer -= Time.deltaTime;
            }
            //If they aren't sprinting or exhausted, and stamina isn't full, refill stamina over time
            else if (_sprintTimer < _maxSprintTimer)
            {
                _lerpValue = _sprintTimer / _maxSprintTimer;
                _staminaBar.sizeDelta = new Vector2(Mathf.Lerp(0, 3.2f, _lerpValue), 100);

                _sprintTimer += Time.deltaTime;
            }

            //If shift is released, stop sprinting
            if (Input.GetKeyUp(KeyCode.LeftShift))
                Sprinting = false;


            if (Input.GetKeyDown(KeyCode.F))
            {
                _flashlight.Toggled = !_flashlight.Toggled;
            }

            //TODO: Pause menu
            //if (Input.GetKeyDown(KeyCode.Escape))
            //{

            //}

            // player movement - forward, backward, left, right
            float horizontal = Input.GetAxis("Horizontal") * _movementSpeed;
            float vertical = Input.GetAxis("Vertical") * _movementSpeed;
            _characterController.Move((_camera.transform.right * horizontal + _camera.transform.forward * vertical) * Time.deltaTime);

            Fall();
        }

        else if (GameBehavior.Instance.CurrentState == GameState.Lose)
            transform.LookAt(_enemy);
    }

    void Fall()
    {
        // Gravity
        if (_characterController.isGrounded)
            _velocity = 0;

        else
        {
            _velocity -= _gravity * Time.deltaTime;
            _characterController.Move(new Vector3(0, _velocity, 0));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected...");
        switch (collision.gameObject.tag)
        {
            case "Pellet":
                Debug.Log("Pellet collision!");
                PelletBehavior.Instance.RemovePellet(collision.gameObject);
                break;

            case "Enemy":
                Debug.Log("Enemy Collision!");
                transform.LookAt(collision.transform);

                _enemy = collision.transform;

                if(!_audio.isPlaying)
                    _audio.Play();

                GameBehavior.Instance.LoseGame();
                break;

            case "Pickup":
                break;
        }
    }
}
