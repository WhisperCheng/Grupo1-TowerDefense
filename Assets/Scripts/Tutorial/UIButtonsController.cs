using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonsController : MonoBehaviour
{
    public List<Image> uiElements;  //Lista de imagenes de los botones

    public void FadeInButton(float duration)
    {
        foreach (Image element in uiElements)
        {
            StartCoroutine(FadeImage(element, element.color.a, 1, duration));
        }
    }

    public void FadeOutButton(float duration)
    {
        foreach (Image element in uiElements)
        {
            StartCoroutine(FadeImage(element, element.color.a, 0, duration));
        }
    }

    private IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color imageColor = image.color;

        while (elapsedTime < duration)
        {
            imageColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        imageColor.a = endAlpha; // Asegurar el valor final
        image.color = imageColor;
    }
}
