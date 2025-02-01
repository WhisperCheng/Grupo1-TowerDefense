using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelMovement : MonoBehaviour
{
  
    [Header("Panel to Move")]
    [SerializeField] private RectTransform panelToMove;

    [Header("Animation Parameters")]
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private LeanTweenType animationType;

    [Header("Button to Listen")]
    [SerializeField] private List<Button> buttons; // Lista de botones

    private Vector2 originalPosition;

    private void OnEnable()
    {
        
        originalPosition = panelToMove.anchoredPosition;
        MovePanelToTargetPosition();
        foreach (Button button in buttons)
        {
                button.onClick.AddListener(OnButtonClick); 
        }
    }

    private void OnDisable()
    {
        
        foreach (Button button in buttons)
        {
            
                button.onClick.RemoveListener(OnButtonClick);
           
        }
    }

    private void MovePanelToTargetPosition()
    {
      
        LeanTween.move(panelToMove, targetPosition, animationDuration).setEase(animationType);
    }

    private void OnButtonClick()
    {
        
        LeanTween.move(panelToMove, originalPosition, animationDuration).setEase(animationType);
    }
}