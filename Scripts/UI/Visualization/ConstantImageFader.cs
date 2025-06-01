using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ConstantImageFader : MonoBehaviour , IVisualFeedback
{
    [SerializeField] bool applyAtStart = true;
    [SerializeField] Image image;
    [SerializeField] float fadeDuration;
    [SerializeField] float alphaStart = 0.3f;
    [SerializeField] float alphaFinish = 0.7f;
    Color standartColor;
    private void Start()
    {
        if (image != null) 
        {
            standartColor = image.color;
            if (applyAtStart)
            {
                StartCoroutine(FadeInOut());
            }
        }
    }
    private IEnumerator FadeInOut()
    {
        while (true)
        {
            yield return StartCoroutine(Fade(alphaStart, alphaFinish));  // Fade in
            yield return StartCoroutine(Fade(alphaFinish, alphaStart));  // Fade out
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = image.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            image.color = color;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }

    public void Perform()
    {
        StartCoroutine(FadeInOut());
    }

    public void Stop()
    {
        StopAllCoroutines();
        image.color = standartColor;
    }
}
