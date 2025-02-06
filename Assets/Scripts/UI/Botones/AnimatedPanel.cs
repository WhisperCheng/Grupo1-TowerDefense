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
    [SerializeField] private float buttonsDelay = 0.6f;
    [SerializeField] private float buttonsAnimationDuration = 0.6f;
    [SerializeField] private Vector3 buttonsInitialPosition;
    [SerializeField] private Vector2 buttonsLocalFinalPosition;
    [SerializeField] private LeanTweenType animationType;
    [SerializeField] private List<Button> buttons;
    private Dictionary<Button,Vector3> initialButtonsPos = new Dictionary<Button, Vector3>();

    [Header("Animations")]
    public bool hadInitialAnimation = false;
    public bool finalAnimation = false;

    private void Awake()
    {
        foreach (Button btn in buttons)
        {
            if (btn != null)
            {
                RectTransform rt = btn.GetComponent<RectTransform>();
                btn.onClick.AddListener(() => OnButtonClick());
                initialButtonsPos.Add(btn, btn.GetComponent<RectTransform>().anchoredPosition3D);
                // anchoredPosition3D importante usarlo en lugar del position o localposition
            }
        }
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        PositionButtonsOnStart();

        originalScale = transform.localScale;

        if (hadInitialAnimation)
            StartCoroutine(DelayedInitialAnimation(buttonsDelay));
    }

    private void OnDisable()
    {
        foreach (Button btn in buttons)
        {
            RectTransform rect = btn.GetComponent<RectTransform>();
            LeanTween.cancel(rect);
            rect.anchoredPosition3D = initialButtonsPos[btn];
        }
    }

    private void PositionButtonsOnStart()
    {
        foreach (Button btn in buttons)
        {
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.anchoredPosition3D = new Vector3(rect.anchoredPosition3D.x + buttonsInitialPosition.x, rect.anchoredPosition3D.y, rect.anchoredPosition3D.z);
        }
    }

    /*private void PositionButtonsOnDisable()
    {
        foreach (Button btn in buttons)
        {
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.position = new Vector3(rect.position.x - buttonsInitialPosition.x, rect.position.y, rect.position.z);
        }
    }*/

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
        foreach (Button btn in buttons)
        {
            if (btn != null)
            {
                ExecuteFinalAnimation(btn, buttonsLocalFinalPosition);
            }
        }
    }

    public void ExecuteInitialAnimation(Button button, Vector3 finalPos)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        LeanTween.moveLocal(button.gameObject, rect.localPosition + finalPos, buttonsAnimationDuration).setEase(animationType);
    }

    public void ExecuteFinalAnimation(Button button, Vector3 finalPos)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        LeanTween.moveLocal(button.gameObject, rect.localPosition - finalPos, buttonsAnimationDuration).setEase(animationType);
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