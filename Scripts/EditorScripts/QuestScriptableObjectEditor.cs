using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(AchievementSO))]
public class QuestScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AchievementSO questSO = (AchievementSO)target;

        if(GUILayout.Button("Reset Progress"))
        {
            questSO.ResetProgress();
            EditorUtility.SetDirty(questSO);
        }
        if(GUILayout.Button("Complete Progress"))
        {
            questSO.CompleteProgress();
            EditorUtility.SetDirty(questSO);
        }
    }
}
#endif