using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [SerializeField] Slider brightness;
    [SerializeField] Image panelBrightness;
    void Start()
    {
        brightness.value = PlayerPrefs.GetFloat("brightness", 0.7f);
        panelBrightness.color = new Color(panelBrightness.color.r, panelBrightness.color.g, panelBrightness.color.b, brightness.value);
    }

    public void ChangeBrightness(float value)
    {
        brightness.value = value;
        PlayerPrefs.SetFloat("brightness", value);
        panelBrightness.color = new Color(panelBrightness.color.r, panelBrightness.color.g, panelBrightness.color.b, value);
    }
    /*public void ChangeVolume(float volume)
    {

    }
    public void ChangeSensitive(float sensitive)
    {

    }
     
     
     */
}

