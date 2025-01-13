using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    [Header("Animation Object")]
    [SerializeField] private GameObject animationObject;

    [Header("Animation Parameters")]
    [SerializeField] private LeanTweenType easeInScale;
    [SerializeField] private float animationInTime;
    [SerializeField] private float animationTimer;
    [SerializeField] Vector2 initialPosition;
    [SerializeField] Vector2 finalPosition;

    public void ExecuteAnimation()
    {
        
        LeanTween.move(gameObject, (finalPosition), animationTimer).setEase(LeanTweenType.easeInBack);
    }
}
