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
    [SerializeField] private float buttonsAnimationDuration = 0.6f;
    [SerializeField] private Vector2 buttonsInitialPosition;
    [SerializeField] private Vector2 panelLocalFinalPosition;
    [SerializeField] private Vector2 buttonsLocalFinalPosition;
    [SerializeField] private LeanTweenType animationType;
    [SerializeField] private List<Button> buttons;

    [Header("Animations")]
    public bool hadInitialAnimation = false;
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
            rect.position = new Vector3(buttonsInitialPosition.x, buttonsInitialPosition.y + rect.position.y, 0);
        }
    }

    private IEnumerator DelayedInitialAnimation(float delay)
    {
        foreach (Button btn in buttons)
        {
            if (btn != null)
            {
                ExecuteInitialAnimation(btn, buttonsLocalFinalPosition);
                
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
        LeanTween.moveLocal(button.gameObject, rect.localPosition + finalPos, buttonsAnimationDuration).setEase(animationType);
    }

    public void ExecuteFinalAnimation()
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        Vector3 panelFinalPos = panelLocalFinalPosition; // Esto es solo para convertir el Vector2 a Vector3
        LeanTween.moveLocal(gameObject, rect.localPosition + panelFinalPos, delay).setEase(animationType);
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
