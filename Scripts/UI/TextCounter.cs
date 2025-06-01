using System.Collections;
using TMPro;
using UnityEngine;

public class TextCounter : MonoBehaviour
{
    [SerializeField] TimeScaleMode timeScaleMode = TimeScaleMode.Scaled;
    [SerializeField] TextMeshProUGUI text;
    public int StartValue;
    public int EndValue;
    public float Duration;
    public AnimationCurve countCurve = AnimationCurve.EaseInOut(0,0,1,1);  
    private bool countingStarted;
    private float timer = 0f;

    private void Update()
    {
        if(countingStarted)
        {
            if(timer <= Duration)
            {
                timer += Time.unscaledDeltaTime;
                float evaluated = countCurve.Evaluate(timer / Duration);
                int value = (int)Mathf.Lerp(StartValue, EndValue, evaluated);
                text.text = value.ToString();
            }
        }
    }
    public void StartCounting(int start , int end , float duration)
    {       
        StartValue = start;
        EndValue = end;
        timer = 0f;
        this.Duration = duration;

        countingStarted = true;
    }

    private IEnumerator Counter(int start , int end , float duration)
    {
        int value = start;

        float t = 0f;
        while (t <= duration)
        {
            if (timeScaleMode == TimeScaleMode.Scaled) 
                t += Time.deltaTime;
            else 
                t += Time.unscaledDeltaTime; 
            
            float evaluated = countCurve.Evaluate(t / duration);
            value = (int)Mathf.Lerp(start, end, evaluated);
            text.text = value.ToString();

            yield return null;
        }
        value = end;
        text.text = value.ToString ();
    }
}
