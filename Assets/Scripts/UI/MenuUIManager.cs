using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    public static MenuUIManager Instance { get; private set; }


    [Header("Animation Parameters")]
    [SerializeField] private LeanTweenType easeInScale;
    [SerializeField] Vector2 initialPosition;
    [SerializeField] Vector2 finalPosition;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    internal void ExecuteAnimation(GameObject gameObject, float delay, Vector2 finalPos)
    {
        LeanTween.move(gameObject, finalPos, delay -0.5f).setEase(LeanTweenType.easeInBack);

    }
}
