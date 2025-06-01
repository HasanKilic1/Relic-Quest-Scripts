using TMPro;
using UnityEngine;
public static class HKDebugger
{
    private static GameObject textPrefab;

    static HKDebugger()
    {
        // Load the text prefab (ensure you have a Resources folder and place your prefab inside it)
        textPrefab = Resources.Load<GameObject>("DebugWorldText");
    }
    public static void Log(string message, string color)
    {
        Debug.Log(FormatMessage(message, color));
    }

    public static void LogError(string message)
    {
        Log(message, "red");
    }

    public static void LogWarning(string message)
    {
        Log(message, "yellow");
    }

    public static void LogSuccess(string message)
    {
        Log(message, "green");
    }

    public static void LogInfo(string message)
    {
        Log(message, "cyan");
    }

    private static string FormatMessage(string message, string color)
    {
        return $"<color={color}>{message}</color>";
    }

    public static void LogWorldText(string message, Vector3 position,  float duration = 2f , float size = 10 , Transform parentTransform = null)
    {
        if (textPrefab == null)
        {
            Debug.LogError("Text prefab not found! Please ensure it is placed in a Resources folder and named 'TextPrefab'.");
            return;
        }

        GameObject textObject = GameObject.Instantiate(textPrefab, position, Quaternion.identity);
        if(parentTransform != null) { textObject.transform.SetParent(parentTransform); }

        TextMeshPro textMesh = textObject.GetComponent<TextMeshPro>();
        textMesh.fontSize = size;

        if (textMesh != null)
        {
            textMesh.text = message;
        }
        else
        {
            Debug.LogError("TextPrefab does not have a TextMesh component!");
        }

        // Destroy the text object after a specified duration
        GameObject.Destroy(textObject, duration);
    }
}

