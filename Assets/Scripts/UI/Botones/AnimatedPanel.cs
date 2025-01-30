using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AnimatedPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static AnimatedPanel Instance { get; private set; }

    [Header("Animation Parameters")]
    [SerializeField] private Vector3 originalScale;
    [SerializeField] private float hoverScaleFactor = 1.1f;
    [SerializeField] private float animationDuration = 0.1f;
    [SerializeField] private float delay = 0.6f;
    [SerializeField] private float buttonsDelay = 0.6f;
    [SerializeField] private Vector2 panelFinalPosition;
    [SerializeField] private Vector2 buttonsRelativeFinalPosition;
    [SerializeField] private LeanTweenType animationType;
    [SerializeField] private List<Button> buttons;
    public bool hadInitialAnimation = false;

    [Header("Final Animation")]
    public bool finalAnimation = false;

    private void Start()
    {
        //hadInitialAnimation = false;

        PositionButtonsOnStart();

        originalScale = transform.localScale;

        if(hadInitialAnimation)
        StartCoroutine(DelayedInitialAnimation(buttonsDelay));

        foreach (Button btn in buttons)
        {
            if (btn != null)
            {
                btn.onClick.AddListener(OnButtonClick);
            }
        }
    }

    private void PositionButtonsOnStart()
    {
        foreach (Button btn in buttons)
        {
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.position = new Vector3(-200, rect.position.y, 0);
        }
    }

    private IEnumerator DelayedInitialAnimation(float delay)
    {
        foreach (Button btn in buttons)
        {
            if (btn != null)
            {
                ExecuteInitialAnimation(btn, buttonsRelativeFinalPosition);
                
            }
            yield return new WaitForSeconds(delay);
        }
    }
    private void OnButtonClick()
    {
        ExecuteFinalAnimation();
    }
    public void ExecuteInitialAnimation(Button button, Vector3 finalPos)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        LeanTween.moveX(button.gameObject, button.transform.position.x + finalPos.x, buttonsDelay);
        LeanTween.moveY(button.gameObject, button.transform.position.y  + finalPos.y, buttonsDelay);
    }

    public void ExecuteFinalAnimation()
    {
        LeanTween.moveLocal(gameObject, panelFinalPosition, delay).setEase(animationType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, originalScale * hoverScaleFactor, animationDuration).setEase(LeanTweenType.easeOutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, originalScale, animationDuration).setEase(LeanTweenType.easeOutQuad);
    }
}
