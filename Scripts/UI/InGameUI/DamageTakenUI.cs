using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTakenUI : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] AnimationCurve imageCurve;
    [SerializeField] float duration;
    bool isPlaying;
    private void OnEnable()
    {
        PlayerHealth.OnDamageTaken += Play;
    }

    private void OnDisable()
    {
        PlayerHealth.OnDamageTaken -= Play;
    }

    private void Start()
    {
        image.enabled = false;
    }

    public void Play(float t)
    {
        if (!isPlaying)
        {
            isPlaying = true;
            image.enabled = true;
            StartCoroutine(Fade());
        }

    }

    private IEnumerator Fade()
    {
        float t = 0f;
        Color color = image.color;
        while(t < duration)
        {
            t += Time.deltaTime;
            float alpha = imageCurve.Evaluate(t / duration);
            color.a = alpha;
            image.color = color;
            yield return null;
        }
        image.enabled = false;
        isPlaying = false;
    }
}
