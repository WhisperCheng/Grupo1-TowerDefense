using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    private enum VolumeType
    {
        MASTER,
        MUSIC,
        SFX,
    }

    [SerializeField]
    private VolumeType volumeType;
    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
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
}
