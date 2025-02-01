using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class OptionsController : MonoBehaviour
{
    [Header("Options")]
   
    [SerializeField] Slider brightness;
    [SerializeField] Image panelBrightness;

    private enum VolumeType
    {
        MASTER,

        MUSIC,

        SFX,
    }
    [SerializeField]
    private VolumeType volumeType;
    private Slider volumeSlider;

    [SerializeField] private Slider sensitiveSlider;
    
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

       
        volumeSlider = GetComponent<Slider>();
    }

    void Start()
    {
         brightness.value = PlayerPrefs.GetFloat("brightness", 0.7f);
        UpdateBrightnessPanel(brightness.value);
        brightness.onValueChanged.AddListener(ChangeBrightness);
    }

    private void Update()
    {
       
        //switch (volumeType)
        //{
           /* case VolumeType.MASTER:
                volumeSlider.value = AudioManager.instance.masterVolume;
                break;
            case VolumeType.MUSIC:
                volumeSlider.value = AudioManager.instance.musicVolume;
                break;
            case VolumeType.SFX:
                volumeSlider.value = AudioManager.instance.SFXVolume;
                break;
            default:
                Debug.LogWarning("Volume type no supported" + volumeType);
                break;*/
        //}
    }
    public void ChangeBrightness(float value)
    {

        PlayerPrefs.SetFloat("brightness", value);
        UpdateBrightnessPanel(value);
    }
    public void ChangeVolume(float volume)
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                AudioManager.instance.masterVolume = volumeSlider.value;
                break;
            case VolumeType.MUSIC:
                AudioManager.instance.musicVolume = volumeSlider.value;
                break;
            case VolumeType.SFX:
                AudioManager.instance.SFXVolume = volumeSlider.value;
                break;
            default:
                Debug.LogWarning("Volume type no supported" + volumeType);
                break;
        }
        PlayerPrefs.SetFloat("volume", volume);
    }
    public void ChangeSensitive(float sensitive)
    {

        sensitiveSlider.value = ThirdPersonCam.instance._turnSpeed;
        PlayerPrefs.SetFloat("sensitive", sensitive);
    }
    private void UpdateBrightnessPanel(float value)
    {
        float alpha = 1f - value;
        alpha = Mathf.Clamp(alpha, 0f, 1f);
        panelBrightness.color = new Color(panelBrightness.color.r, panelBrightness.color.g, panelBrightness.color.b, alpha);
    }
}

