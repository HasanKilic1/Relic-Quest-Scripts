using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.UI.Visualization
{
    public class ImageColorizer : MonoBehaviour , IVisualFeedback
    {
        public bool ReturnNormalAfterFinish;
        public Image targetImage; 
        public Color StartColor;
        public Color EndColor;
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float Duration;

        private Color normalColor;
        private void Start()
        {
            normalColor = targetImage.color;
        }
        public void Perform()
        {
            StartCoroutine(ColorRoutine(StartColor , EndColor , Duration));
        }

        public void Colorize(Color start, Color end , float duration) 
        {
            StartCoroutine(ColorRoutine(start, end, duration));
        }
        private IEnumerator ColorRoutine(Color start, Color end , float duration)
        {
            float t = 0f;
            while (t < duration / 2) 
            {
                t += Time.deltaTime;
                float eval = curve.Evaluate(t / Duration);
                targetImage.color = Color.Lerp(start, end, eval);
                yield return null;
            }

            t = 0f;
            while (t < duration / 2)
            {
                t += Time.deltaTime;
                float eval = curve.Evaluate(t / Duration);
                targetImage.color = Color.Lerp(end, start, eval);
                yield return null;
            }
            if (ReturnNormalAfterFinish) { targetImage.color = normalColor; }
        }

        public void Stop()
        {
            StopAllCoroutines();
            targetImage.color = normalColor;
        }
    }
}