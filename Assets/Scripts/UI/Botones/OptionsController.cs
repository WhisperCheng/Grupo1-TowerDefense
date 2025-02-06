using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class OptionsController : MonoBehaviour
{
    [Header("Options")]
   
    [SerializeField] public Slider brightnessSlider;
    [SerializeField] public Image panelBrightness;

    [SerializeField] public Slider sensitiveSlider;

    public static OptionsController instance { get; private set; }

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        if (brightnessSlider != null)
        {
            brightnessSlider.value = PlayerPrefs.GetFloat("brightness", 0.5f);
            brightnessSlider.onValueChanged.AddListener(ChangeBrightness);
        }
        
        if (sensitiveSlider != null)
        {
            sensitiveSlider.value = PlayerPrefs.GetFloat("sensitive", 0.5f);
            sensitiveSlider.onValueChanged.AddListener(ChangeSensitive);
        }
        //UpdateBrightnessPanel(brightness.value);


    }

    private void Update()
    {
        
    }

    public void ChangeBrightness(float brightness)
    {

        UpdateBrightnessPanel(brightness);
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            
            PlayerPrefs.SetFloat("brightness", brightnessSlider.value);
            
        }
        else
        {
            
            brightnessSlider.value = PlayerPrefs.GetFloat("brightness", 0.5f);
        }
        PlayerPrefs.SetFloat("brightness", brightness);
    }

    public void ChangeSensitive(float sensitive)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            
            PlayerPrefs.SetFloat("sensitive", sensitiveSlider.value);
           
        }
        else
        {
            
            sensitiveSlider.value = PlayerPrefs.GetFloat("sensitive", 40f);
            ThirdPersonCam.instance._turnSpeed = sensitiveSlider.value * 80f;
        }
        PlayerPrefs.SetFloat("sensitive", sensitive);
    }

    private void UpdateBrightnessPanel(float value)
    {
        float alpha = 1f - value;
        alpha = Mathf.Clamp(alpha, 0f, 1f);
        panelBrightness.color = new Color(panelBrightness.color.r, panelBrightness.color.g, panelBrightness.color.b, alpha);
    }
}
