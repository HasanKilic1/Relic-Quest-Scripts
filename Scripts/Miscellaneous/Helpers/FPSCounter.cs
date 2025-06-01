using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;  // Reference to the TextMeshProUGUI component
    public float updateInterval = 0.5f;  // Time between updates (in seconds)

    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    private float timeSinceLastUpdate = 0.0f;

    void Update()
    {
        // Calculate delta time for current frame
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;

        // Increment time since last update
        timeSinceLastUpdate += Time.unscaledDeltaTime;

        // Update the text at the specified interval
        if (timeSinceLastUpdate >= updateInterval)
        {
            fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
            timeSinceLastUpdate = 0.0f;  // Reset timer
        }
    }
}
