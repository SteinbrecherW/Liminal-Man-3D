using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] Image _image;

    [SerializeField] Sprite _selectedSprite;
    [SerializeField] Sprite _deselectedSprite;

    [SerializeField] string _scene;

    void Start()
    {
        _image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (MenuManager.Instance.CurrentState == MenuManager.MenuState.Active)
            _image.sprite = _selectedSprite;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (MenuManager.Instance.CurrentState == MenuManager.MenuState.Active)
            _image.sprite = _deselectedSprite;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        MenuManager.Instance.FadeOut(_scene);
    }
}
