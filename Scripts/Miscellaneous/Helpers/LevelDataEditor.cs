using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GridSO))]
public class LevelDataEditor : Editor
{
    private bool[,] buttonPressedStates;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GridSO gridSO = (GridSO)target;

        // Dropdown to select the object to place
        gridSO.SELECT_OBJECT_ID = EditorGUILayout.Popup("Select Object", gridSO.SELECT_OBJECT_ID, gridSO.GridObjects.Select(go => go.ObjectType.ToString()).ToArray());

        if (GUILayout.Button("Clear Coordinates"))
        {
            ClearAllPressedButtons();
            gridSO.ClearCoordinates();
        }

        if(GUILayout.Button("Setup Randomly"))
        {
            gridSO.SetupRandomly();
        }

        DrawGridMap(gridSO);
        DrawFilledCoordinates(gridSO);
    }

    private void DrawGridMap(GridSO gridSO)
    {
        int gridSizeX = gridSO.XMax;
        int gridSizeZ = gridSO.ZMax;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Grid Map");
        GUILayout.BeginVertical(GUI.skin.box);

        InitializeButtonPressedStates(gridSizeX, gridSizeZ);

        for (int z = gridSizeZ - 1; z >= 0; z--)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < gridSizeX; x++)
            {
                Vector2Int coord = new Vector2Int(x, z);
                bool isPressed = buttonPressedStates[x, z];
                GUIStyle buttonStyle = isPressed ? GetPressedButtonStyle() : GUI.skin.button;

                string buttonText = gridSO.ObjectAndCoordinates.Any(obj => obj.Coordinate == coord)
                    ? $"({x},{z}) {gridSO.ObjectAndCoordinates.First(obj => obj.Coordinate == coord).GridObject.ObjectType}"
                    : $"({x},{z})";

                if (GUILayout.Button(buttonText, buttonStyle))
                {
                    // Toggle the pressed state of the button
                    buttonPressedStates[x, z] = !isPressed;

                    if (buttonPressedStates[x, z])
                    {
                        gridSO.AddObjectToCoordinate(coord);
                    }
                    else
                    {
                        gridSO.ObjectAndCoordinates.RemoveAll(obj => obj.Coordinate == coord);
                    }

                    EditorUtility.SetDirty(gridSO); // Mark the scriptable object as dirty so changes are saved
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    private void DrawFilledCoordinates(GridSO gridSO)
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Filled Coordinates:");

        var groupedCoordinates = gridSO.ObjectAndCoordinates
            .GroupBy(obj => obj.GridObject.ObjectType)
            .ToDictionary(group => group.Key, group => group.Select(obj => obj.Coordinate).ToList());

        foreach (var group in groupedCoordinates)
        {
            EditorGUILayout.LabelField($"{group.Key} Coordinates:");
            foreach (var coord in group.Value)
            {
                EditorGUILayout.LabelField($"  - {coord}");
            }
        }
    }

    private void InitializeButtonPressedStates(int sizeX, int sizeY)
    {
        if (buttonPressedStates == null || buttonPressedStates.GetLength(0) != sizeX || buttonPressedStates.GetLength(1) != sizeY)
        {
            buttonPressedStates = new bool[sizeX, sizeY];
        }
    }

    private GUIStyle GetPressedButtonStyle()
    {
        GUIStyle pressedButtonStyle = new GUIStyle(GUI.skin.button);
        pressedButtonStyle.normal.textColor = Color.red;
        return pressedButtonStyle;
    }

    private void ClearAllPressedButtons()
    {
        if (buttonPressedStates != null)
        {
            for (int x = 0; x < buttonPressedStates.GetLength(0); x++)
            {
                for (int y = 0; y < buttonPressedStates.GetLength(1); y++)
                {
                    buttonPressedStates[x, y] = false;
                }
            }
        }
    }
}
#endif