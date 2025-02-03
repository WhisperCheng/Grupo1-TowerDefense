using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Slot : MonoBehaviour
{
    public Color colorOnSelectedButton;
    public UnityEvent OnSlotSelected;
    private Color _normalButtonColor;
    private Button _currentButton;
    private Image _buttonImage;

    public float TransitionTime { get; set; }
    public float OnSelectedScale { get; set; }
    public bool Selected { get; set; }
    public Button Button { get { return _currentButton; } }
    public LeanTweenType AnimationType { get; set; }
    // Start is called before the first frame update

    private void Awake()
    {
        _currentButton = GetComponent<Button>();
        _normalButtonColor = _currentButton.colors.normalColor;
        _buttonImage = GetComponent<Image>();
    }
    void Start()
    {

    }

    public void ToggleHighlight()
    {
        LeanTween.cancel(_buttonImage.rectTransform);
        Selected = !Selected;

        Color currentColor = Selected ? colorOnSelectedButton : _normalButtonColor;
        float currentScale = Selected ? OnSelectedScale : 1;

        LeanTween.scale(_buttonImage.rectTransform, Vector3.one * currentScale, TransitionTime).setEase(AnimationType);
        LeanTween.color(_buttonImage.rectTransform, currentColor, TransitionTime).setEase(AnimationType);

        if (Selected) SelectButton();
        //GetComponent<Image>().color = colorOnSelectedButton;
    }

    private void SelectButton()
    {
        OnSlotSelected.Invoke();
        //PlaceManager.Instance.OnClickButtons();
        //_currentButton.onClick.Invoke();
    }
}
