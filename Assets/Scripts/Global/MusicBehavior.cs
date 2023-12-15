using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBehavior : MonoBehaviour
{
    enum State
    {
        Hub,
        Game
    }
    State _currentState;

    [SerializeField] AudioSource _audio;
    [SerializeField] AudioClip[] _clips;

    void Start()
    {
        _currentState = State.Hub;

        _audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(_currentState == State.Hub && !_audio.isPlaying)
        {
            _audio.clip = _clips[0];
            _audio.Play();
        }
        else if (!_audio.isPlaying)
        {
            _audio.clip = _clips[1];
            _audio.Play();
        }

        if(MenuManager.Instance.CurrentState == MenuManager.MenuState.FadeOut)
        {

        }    
    }
}
