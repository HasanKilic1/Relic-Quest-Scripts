using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextMeshProFader : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    public Color fadeInColor = new Color(1f, 1f, 1f, 1f); // Fully opaque white
    public Color fadeOutColor = new Color(1f, 1f, 1f, 0f); // Fully transparent white
    public float delayBetweenFades = 0.5f;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            textMeshPro.color = Color.Lerp(fadeOutColor, fadeInColor, elapsedTime / fadeInDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        textMeshPro.color = fadeInColor;

        yield return new WaitForSecondsRealtime(delayBetweenFades);

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeOutDuration)
        {
            textMeshPro.color = Color.Lerp(fadeInColor, fadeOutColor, elapsedTime / fadeOutDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        textMeshPro.color = fadeOutColor;

        yield return new WaitForSecondsRealtime(delayBetweenFades);

        StartCoroutine(FadeIn());
    }
}
