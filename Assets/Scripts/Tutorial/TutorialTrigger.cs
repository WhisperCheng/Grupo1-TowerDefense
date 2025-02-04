using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    //Asegura que el trigger se activa solo una vez
    private bool hasActivated = false;
    public UnityEvent pachamamaEvent, enemiesEvent;
    public List<Image> elementsActivated;
    public float fadeDuration = 1f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasActivated)
        {
            hasActivated = true;
            TutorialController.Instance.ActivateModule();
            pachamamaEvent.Invoke();
            StartCoroutine(FadeInButtons());
            Destroy(gameObject);
        }

        if (other.CompareTag("Enemy"))
        {
            enemiesEvent.Invoke();
        }
    }

    private IEnumerator FadeInButtons()
    {
        foreach (Image button in elementsActivated)
        {
            StartCoroutine(FadeImage(button, button.color.a, 1, fadeDuration));
        }
        yield return null;
    }

    private IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = image.color;

        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime/duration);
            image.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }
}
