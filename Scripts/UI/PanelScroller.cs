using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelScroller : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;

    [Range(-1,1)][SerializeField] float scrollSensivity;
    [SerializeField] float scrollTime;
    float currentValue;
    float targetValue;
    public void ScrollRight()
    {
        if(targetValue == 1)
        {
            targetValue = 0;
            StartCoroutine(ScrollRoutine(targetValue));
            return;
        }
        targetValue = currentValue + scrollSensivity;
        if (targetValue > 1) targetValue = 1;
        
        StartCoroutine(ScrollRoutine(targetValue));        
    }

   
    private IEnumerator ScrollRoutine(float targetValue)
    {
        float diff = targetValue - currentValue;
        float changePerSec  = diff / scrollTime;
        this.GetComponent<Button>().interactable = false;
        float t = 0f;
        while (t < scrollTime)
        {
            t += Time.deltaTime;
            currentValue += changePerSec * Time.deltaTime;
            scrollRect.horizontalNormalizedPosition = currentValue;
            yield return null;  
        }
        this.GetComponent<Button>().interactable = true;
    }
}
