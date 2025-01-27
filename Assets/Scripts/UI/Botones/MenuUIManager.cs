using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuUIManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static MenuUIManager Instance { get; private set; }

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

    [Header("Animation Parameters")]
    [SerializeField] private LeanTweenType animationButton;
    Vector2 finalPosition;
    [SerializeField] Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;

    }

    internal void ExecuteAnimation(GameObject gameObject, float delay, Vector2 finalPos)
    {
        LeanTween.move(gameObject, finalPos, delay -0.5f).setEase(animationButton);

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, originalScale * 1.1f, 0.1f).setEase(LeanTweenType.easeOutQuad);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, originalScale, 0.1f).setEase(LeanTweenType.easeOutQuad);

    }
}
