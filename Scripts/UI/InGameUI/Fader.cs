using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [SerializeField] private float fadeInDelay = 1.5f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.2f;
    [SerializeField] Image image;

    private void Start()
    {
        FadeOut();
    }

    private void FadeOut()
    {
        StartCoroutine(FadeRoutine(0f, fadeOutDuration, 1f, 0f));
    }

    private void FadeIn()
    {
        StartCoroutine(FadeRoutine(fadeInDelay, fadeInDuration, 0f, 1f));
    }

    private IEnumerator FadeRoutine(float delay , float duration , float startAlpha , float targetAlpha)
    {
        image.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float alpha = startAlpha;
        Color imageColor = image.color;
        imageColor.a = alpha;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            alpha = Mathf.Lerp(alpha , targetAlpha , elapsed / duration);
            Color toColor = new(imageColor.r, imageColor.g, imageColor.b, alpha);
            image.color = Color.Lerp(imageColor, toColor, elapsed / duration); 
            yield return null;
        }
        image.gameObject.SetActive(false);
    }
}
