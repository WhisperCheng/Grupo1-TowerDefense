using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class CambiarIdioma : MonoBehaviour
{
    int idiomaActual = 0;
    List<Locale> listaIdiomas;
    void Start()
    {
        listaIdiomas = LocalizationSettings.AvailableLocales.Locales;
    }

    public void CambiarIdiomaArriba()
    {
        idiomaActual = (idiomaActual + 1) % listaIdiomas.Count;
        LocalizationSettings.SelectedLocale = listaIdiomas[idiomaActual++];
        
        
    }
    public void CambiarIdiomaAbajo()
    {

        idiomaActual = (idiomaActual - 1 + listaIdiomas.Count) % listaIdiomas.Count;
        LocalizationSettings.SelectedLocale = listaIdiomas[idiomaActual];
        
    }

}

