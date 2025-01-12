using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider volumen;
    void Start()
    {
        if(PlayerPrefs.HasKey("musicvolumen"))
        {
            PlayerPrefs.SetFloat("musicvolumen", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    public void ChangeVolumen()
    {
        AudioListener.volume = volumen.value;
        Save();
    }
    private void Load()
    {
        volumen.value = PlayerPrefs.GetFloat("musicvolumen");
    }
    private void Save()
    {
        PlayerPrefs.SetFloat("musicvolumen", volumen.value);
    }
}
