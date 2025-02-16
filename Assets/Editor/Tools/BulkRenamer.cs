using UnityEditor;
using UnityEngine;

public class BulkRenamer : EditorWindow
{
    private string newBaseName = "Object_";

    [MenuItem("Tools/Bulk Renamer")]
    public static void ShowWindow()
    {
        GetWindow<BulkRenamer>("Bulk Renamer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Rename Selected Objects", EditorStyles.boldLabel);
        newBaseName = EditorGUILayout.TextField("Base Name:", newBaseName);
        
        if (GUILayout.Button("Rename Selected"))
        {
            RenameObjects();
        }
    }

    private void RenameObjects()
    {
        if (Selection.gameObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected!");
            return;
        }

        int counter = 1;
        foreach (GameObject obj in Selection.gameObjects)
        {
            obj.name = $"{newBaseName}{counter.ToString("00")}";
            counter++;
        }
    }
}