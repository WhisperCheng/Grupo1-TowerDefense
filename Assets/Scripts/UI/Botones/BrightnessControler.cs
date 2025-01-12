using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessControler : MonoBehaviour
{
     public Scrollbar brightness;
    public float brightnessValue;
    public Image panelBrightness;
    void Start()
    {
        brightness.value = PlayerPrefs.GetFloat("brillo", 0.5f);

        panelBrightness.color = new Color(panelBrightness.color.r, panelBrightness.color.g, panelBrightness.color.b, brightnessValue);
    }

    
    void Update()
    {
        
    }
    public void ChangeScrollbar(float value)
    {
        brightnessValue = value;
        PlayerPrefs.SetFloat("brillo", value);
        panelBrightness.color = new Color(panelBrightness.color.r, panelBrightness.color.g, panelBrightness.color.b, brightnessValue);
    }
}
