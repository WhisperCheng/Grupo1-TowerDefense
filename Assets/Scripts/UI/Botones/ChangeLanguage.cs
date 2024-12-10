using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class CambiarIdioma : MonoBehaviour
{



    public void ActualizarIdioma(int indexIdioma)
    {

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[indexIdioma];
    }
}

