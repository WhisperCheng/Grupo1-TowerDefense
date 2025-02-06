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
    public UnityEvent OnSlotDeselected;
    private Color _normalButtonColor;
    private Button _currentButton;
    private Image _buttonImage;

    public float TransitionTime { get; set; }
    public float OnSelectedScale { get; set; }
    public bool Selected { get; set; }
    public bool buttonEnabled = true;
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
        if (!buttonEnabled)
        {
            Color newColor = _buttonImage.color;
            newColor.a = 0;
            _buttonImage.color = newColor;
        }
    }

    public void ToggleHighlight()
    {
        LeanTween.cancel(_buttonImage.rectTransform);
        Selected = !Selected;
        AnimateButton();

        //SelectButton();
        //GetComponent<Image>().color = colorOnSelectedButton;
        if (Selected)  SelectButton(); 
        if (!Selected) { OnSlotDeselected.Invoke(); }
    }

    public void EnableButton()
    {
        buttonEnabled = true;
        StartCoroutine(FadeImage(_buttonImage, 0, 1, 0.5f));
    }

    private void AnimateButton()
    {
        Color currentColor = Selected ? colorOnSelectedButton : _normalButtonColor;
        float currentScale = Selected ? OnSelectedScale : 1;

        LeanTween.scale(_buttonImage.rectTransform, Vector3.one * currentScale, TransitionTime).setEase(AnimationType);
        LeanTween.color(_buttonImage.rectTransform, currentColor, TransitionTime).setEase(AnimationType);
    }

    public void SelectButton()
    {
        if(Selected)
            OnSlotSelected.Invoke();
    }

    private IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = image.color;

        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            image.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }
}
