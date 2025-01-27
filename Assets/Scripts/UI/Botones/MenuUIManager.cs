using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuUIManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public static MenuUIManager Instance { get; private set; }

    [Header("Animation Parameters")]
    [SerializeField] private LeanTweenType animationButton;
    [SerializeField] private Vector2 finalPosition;
    [SerializeField] private Vector3 originalScale;
    [SerializeField] private bool hasInitialAnimation = false; 
    [SerializeField] private float animationDuration = 0.5f; 

    private bool animationCompleted = false; 

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        originalScale = transform.localScale;

        
        if (hasInitialAnimation)
        {
            ExecuteInitialAnimation();
        }
    }

    private void ExecuteInitialAnimation()
    {
        LeanTween.move(gameObject, finalPosition, animationDuration).setEase(animationButton).setOnComplete(() =>
        {
            animationCompleted = true; 
        });
    }

    internal void ExecuteAnimation(GameObject gameObject, float delay, Vector2 finalPos)
    {
        LeanTween.move(gameObject, finalPos, delay).setEase(animationButton);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, originalScale * 1.1f, 0.1f).setEase(LeanTweenType.easeOutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, originalScale, 0.1f).setEase(LeanTweenType.easeOutQuad);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       
        if (!hasInitialAnimation || animationCompleted)
        {
            ExecuteClickAnimation();
        }
    }

    private void ExecuteClickAnimation()
    {
       
        LeanTween.move(gameObject, finalPosition, animationDuration).setEase(animationButton);
    }
}