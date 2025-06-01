#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelDesignEditor : EditorWindow
{
    private GameObject environmentObject;
    private float brushSize = 1.0f;
    private int numberOfObjects = 10;
    private Vector2 rotationRange = new Vector2(0, 360);
    private Vector2 scaleRange = new Vector2(0.5f, 1.5f);

    private bool spawnRandomly = true;
    private Vector3 specifiedRotation = Vector3.zero;
    private Vector3 specifiedScale = Vector3.one;

    private Transform parentTransform;

    private float distanceInterval = 1.0f;
    private Vector3 lineStartPosition = Vector3.zero;
    private Vector3 lineEndPosition = Vector3.right * 10.0f;

    private Vector3 brushPosition = Vector3.zero;

    private List<GameObject> lastSpawnedObjects = new List<GameObject>();
    private Mesh environmentMesh;

    [MenuItem("Tools/Level Designer")]
    public static void ShowWindow()
    {
        GetWindow<LevelDesignEditor>("Level Designer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Designer", EditorStyles.boldLabel);

        environmentObject = (GameObject)EditorGUILayout.ObjectField("Environment Object", environmentObject, typeof(GameObject), false);

        parentTransform = (Transform)EditorGUILayout.ObjectField("Parent Transform", parentTransform, typeof(Transform), true);

        brushSize = EditorGUILayout.Slider("Brush Size", brushSize, 0.1f, 25.0f);
        numberOfObjects = EditorGUILayout.IntSlider("Number of Objects", numberOfObjects, 1, 100);

        spawnRandomly = EditorGUILayout.Toggle("Spawn Randomly", spawnRandomly);

        if (spawnRandomly)
        {
            rotationRange = EditorGUILayout.Vector2Field("Rotation Range", rotationRange);
            scaleRange = EditorGUILayout.Vector2Field("Scale Range", scaleRange);
        }
        else
        {
            specifiedRotation = EditorGUILayout.Vector3Field("Rotation", specifiedRotation);
            specifiedScale = EditorGUILayout.Vector3Field("Scale", specifiedScale);
        }

        GUILayout.Space(10);
        GUILayout.Label("Line Placement", EditorStyles.boldLabel);
        distanceInterval = EditorGUILayout.FloatField("Distance Interval", distanceInterval);
        lineStartPosition = EditorGUILayout.Vector3Field("Line Start Position", lineStartPosition);
        lineEndPosition = EditorGUILayout.Vector3Field("Line End Position", lineEndPosition);

        if (GUILayout.Button("Place Objects Randomly"))
        {
            PlaceObjects(PlacementType.Random);
        }

        if (GUILayout.Button("Place Objects in Line"))
        {
            PlaceObjects(PlacementType.Line);
        }

        if (GUILayout.Button("Remove Last Spawned Objects"))
        {
            RemoveLastSpawnedObjects();
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        // Allow the user to move the brush in the scene view
        brushPosition = Handles.PositionHandle(brushPosition, Quaternion.identity);

        // Draw the brush area
        Handles.color = Color.green;
        Handles.DrawWireDisc(brushPosition, Vector3.up, brushSize);

        // Draw the line placement preview
        Handles.color = Color.red;
        Handles.DrawLine(lineStartPosition, lineEndPosition);
    }

    private void DrawPreview()
    {
        if (environmentObject == null)
            return;

        // Calculate and draw placeholders for spawned objects
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 randomPosition = brushPosition + Random.insideUnitSphere * brushSize;
            Quaternion rotation = spawnRandomly ? GetRandomRotation() : Quaternion.Euler(specifiedRotation);
            Vector3 scale = spawnRandomly ? GetRandomScale() : specifiedScale;
            Gizmos.DrawMesh(GetEnvironmentMesh(), randomPosition, rotation, scale);
        }
    }

    private Mesh GetEnvironmentMesh()
    {
        if (environmentMesh == null && environmentObject != null)
        {
            MeshFilter meshFilter = environmentObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
                environmentMesh = meshFilter.sharedMesh;
        }
        return environmentMesh;
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private enum PlacementType
    {
        Random,
        Line
    }

    private void PlaceObjects(PlacementType placementType)
    {
        if (environmentObject == null)
        {
            Debug.LogError("Environment Object is not assigned.");
            return;
        }

        lastSpawnedObjects.Clear();

        if (placementType == PlacementType.Random)
        {
            for (int i = 0; i < numberOfObjects; i++)
            {
                Vector3 randomPosition = GetRandomPositionWithinBrush();
                Quaternion rotation = spawnRandomly ? GetRandomRotation() : Quaternion.Euler(specifiedRotation);
                Vector3 scale = spawnRandomly ? GetRandomScale() : specifiedScale;

                GameObject instance = Instantiate(environmentObject, randomPosition, rotation, parentTransform);
                instance.transform.localScale = scale;
                lastSpawnedObjects.Add(instance);
            }
        }
        else if (placementType == PlacementType.Line)
        {
            PlaceObjectsInLine();
        }
    }

    private void PlaceObjectsInLine()
    {
        float distance = Vector3.Distance(lineStartPosition, lineEndPosition);
        int numberOfPositions = Mathf.FloorToInt(distance / distanceInterval) + 1;
        Vector3 direction = (lineEndPosition - lineStartPosition).normalized;

        for (int i = 0; i < numberOfPositions; i++)
        {
            Vector3 position = lineStartPosition + direction * distanceInterval * i;
            Quaternion rotation = spawnRandomly ? GetRandomRotation() : Quaternion.Euler(specifiedRotation);
            Vector3 scale = spawnRandomly ? GetRandomScale() : specifiedScale;

            GameObject instance = Instantiate(environmentObject, position, rotation, parentTransform);
            instance.transform.localScale = scale;
            lastSpawnedObjects.Add(instance);
        }
    }

    private void RemoveLastSpawnedObjects()
    {
        foreach (GameObject obj in lastSpawnedObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        lastSpawnedObjects.Clear();
    }

    private Vector3 GetRandomPositionWithinBrush()
    {
        Vector2 randomPoint = Random.insideUnitCircle * brushSize;
        return new Vector3(brushPosition.x + randomPoint.x, brushPosition.y, brushPosition.z + randomPoint.y);
    }

    private Quaternion GetRandomRotation()
    {
        float randomYRotation = Random.Range(rotationRange.x, rotationRange.y);
        return Quaternion.Euler(0, randomYRotation, 0); // Rotating only around the Y axis
    }

    private Vector3 GetRandomScale()
    {
        float randomScale = Random.Range(scaleRange.x, scaleRange.y);
        return new Vector3(randomScale, randomScale, randomScale);
    }
}
#endif