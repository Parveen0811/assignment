
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ObstacleEditor : EditorWindow
{
    private ObstacleData obstacleData;

    [MenuItem("Tools/Obstacle Editor")]
    public static void ShowWindow()
    {
        GetWindow<ObstacleEditor>("Obstacle Editor");
    }

    private void OnGUI()
    {
        obstacleData = (ObstacleData)EditorGUILayout.ObjectField("Obstacle Data", obstacleData, typeof(ObstacleData), false);

        if (obstacleData == null) return;

        for (int y = 0; y < 10; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < 10; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                bool isObstacle = obstacleData.obstaclePositions.Contains(pos);

                bool newState = GUILayout.Toggle(isObstacle, "");
                
                if (newState && !isObstacle)
                    obstacleData.obstaclePositions.Add(pos);
                else if (!newState && isObstacle)
                    obstacleData.obstaclePositions.Remove(pos);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(obstacleData);
            AssetDatabase.SaveAssets();
        }
    }
}
