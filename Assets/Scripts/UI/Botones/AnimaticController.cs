using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimaticController : MonoBehaviour
{
    [SerializeField] private TMP_Text textToFade; 
    [SerializeField] private float fadeSpeed = 0.15f;

    private bool isFading = true;

    private void Start()
    {
            
            Color textColor = textToFade.color;
            textColor.a = 0;
            textToFade.color = textColor;
        
        
    }

    private void Update()
    {
        if (isFading && textToFade != null)
        {
            
            Color textColor = textToFade.color;
            textColor.a += fadeSpeed * Time.deltaTime;

           
            if (textColor.a >= 1)
            {
                textColor.a = 1;
                isFading = false;
            }

            textToFade.color = textColor;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
           LevelChanges.OpenMainMenu();
        }
    }
}
