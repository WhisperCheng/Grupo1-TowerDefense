using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AnimationButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static AnimationButton Instance { get; private set; }

    [Header("Animation Parameters")]
    [SerializeField] private Vector3 originalScale;
    [SerializeField] private float hoverScaleFactor = 1.1f;
    [SerializeField] private float animationDuration = 0.1f;
    [SerializeField] private float delay = 0.6f;
    [SerializeField] private Vector2 finalPosition;
    [SerializeField] private LeanTweenType animationType;
    public bool hadInitialAnimation = false;

    [Header("Final Animation")]
    public bool finalAnimation = false; 

    private void Start()
    {
        hadInitialAnimation = false;

        originalScale = transform.localScale;
        StartCoroutine(DelayedInitialAnimation(2f));

         Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private IEnumerator DelayedInitialAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        ExecuteInitialAnimation();
    }
       private void OnButtonClick()
    {
        ExecuteFinalAnimation(); 
    }
    public void ExecuteInitialAnimation()
    {
        hadInitialAnimation = true;
        LeanTween.moveLocal(gameObject, finalPosition, delay).setEase(animationType);
    }

    public void ExecuteFinalAnimation()
    {

            LeanTween.moveLocal(gameObject, finalPosition, delay).setEase(animationType);
        
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
