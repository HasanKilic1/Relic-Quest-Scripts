using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(RewardSO))]
public class RewardSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);

        GUILayout.Label("Use random option to give a random item or resource", EditorStyles.boldLabel, GUILayout.Width(500), GUILayout.Height(30));

        GUILayout.Space(10);

        DrawDefaultInspector();
    }
}
#endif
