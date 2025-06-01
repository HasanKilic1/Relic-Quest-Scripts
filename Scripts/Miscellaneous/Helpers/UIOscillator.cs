using System.Collections;
using UnityEngine;

public class UIOscillator : MonoBehaviour
{
    public RectTransform uiElement;  // The UI element's RectTransform
    public Vector2 startPosition;
    public Vector2 endPosition;
    public float speed = 1.0f;

    private bool movingToEnd = true;

    void Start()
    {
        // Initialize the start position
        if (uiElement == null)
        {
            uiElement = GetComponent<RectTransform>();
        }

        startPosition = uiElement.anchoredPosition;
    }

    void Update()
    {
        // Use unscaled delta time for time-independent movement
        float unscaledDeltaTime = Time.unscaledDeltaTime;

        if (movingToEnd)
        {
            // Move towards the end position
            uiElement.anchoredPosition = Vector2.MoveTowards(uiElement.anchoredPosition, endPosition, speed * unscaledDeltaTime);

            // Check if we reached the end position
            if (Vector2.Distance(uiElement.anchoredPosition, endPosition) < 0.01f)
            {
                movingToEnd = false; // Start moving back to the start position
            }
        }
        else
        {
            // Move towards the start position
            uiElement.anchoredPosition = Vector2.MoveTowards(uiElement.anchoredPosition, startPosition, speed * unscaledDeltaTime);

            // Check if we reached the start position
            if (Vector2.Distance(uiElement.anchoredPosition, startPosition) < 0.01f)
            {
                movingToEnd = true; // Start moving back to the end position
            }
        }
    }
}