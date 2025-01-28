using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
        panelBrightness.color = new Color(panelBrightness.color.r, panelBrightness.color.g, panelBrightness.color.b, brightness.value);
    }

    private void Update()
    {
       
        switch (volumeType)
        {
            case VolumeType.MASTER:
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
                break;
        }
    }
    public void ChangeBrightness(float value)
    {
        brightness.value = value;
        PlayerPrefs.SetFloat("brightness", value);
        panelBrightness.color = new Color(panelBrightness.color.r, panelBrightness.color.g, panelBrightness.color.b, value);
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
    }
    /*public void ChangeSensitive(float sensitive)
    {

    }
     
     
     */
}

