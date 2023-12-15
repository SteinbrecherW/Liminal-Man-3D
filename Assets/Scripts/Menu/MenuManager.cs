using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] Image _fadeInImage;

    [SerializeField] float _fadeInTime = 3;

    [SerializeField] GameObject _rules;

    public enum MenuState
    {
        FadeIn,
        Active,
        Rules,
        FadeOut
    }
    public MenuState CurrentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        CurrentState = MenuState.FadeIn;

        _rules.SetActive(false);

        StartCoroutine(FadeInImage());

        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if(CurrentState == MenuState.Rules && Input.GetKeyDown(KeyCode.Escape))
        {
            _rules.SetActive(false);
            CurrentState = MenuState.Active;
        }
    }

    public void FadeOut(string scene)
    {
        CurrentState = MenuState.FadeOut;
        StartCoroutine(FadeOutImage(scene));
    }

    IEnumerator FadeInImage()
    {
        _fadeInImage.CrossFadeAlpha(0, _fadeInTime, false);
        yield return new WaitForSeconds(_fadeInTime);
        CurrentState = MenuState.Active;
        _fadeInImage.enabled = false;
    }

    IEnumerator FadeOutImage(string scene)
    {
        if(scene == "Rules")
        {
            //TODO: Rules menu
            _rules.SetActive(true);
            CurrentState = MenuState.Rules;
        }
        else
        {
            _fadeInImage.enabled = true;
            _fadeInImage.CrossFadeAlpha(1, _fadeInTime, false);
            yield return new WaitForSeconds(_fadeInTime);

            if (scene == "Exit")
                Application.Quit();
            else
                SceneManager.LoadScene(scene);
        }
    }
}
