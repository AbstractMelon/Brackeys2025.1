using UnityEditor;
using UnityEngine;

public class TransformSnapshot : EditorWindow
{
    private Vector3 savedPosition;
    private Quaternion savedRotation;
    private Vector3 savedScale;

    [MenuItem("Tools/Transform Snapshot")]
    public static void ShowWindow()
    {
        GetWindow<TransformSnapshot>("Transform Snapshot");
    }

    private void OnGUI()
    {
        GUILayout.Label("Save/Load Transform:", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Save Current Transform"))
        {
            SaveTransform();
        }
        
        if (GUILayout.Button("Load Saved Transform"))
        {
            LoadTransform();
        }
    }

    private void SaveTransform()
    {
        if (Selection.activeTransform)
        {
            savedPosition = Selection.activeTransform.position;
            savedRotation = Selection.activeTransform.rotation;
            savedScale = Selection.activeTransform.localScale;
        }
    }

    private void LoadTransform()
    {
        if (Selection.activeTransform)
        {
            Undo.RecordObject(Selection.activeTransform, "Load Transform");
            Selection.activeTransform.position = savedPosition;
            Selection.activeTransform.rotation = savedRotation;
            Selection.activeTransform.localScale = savedScale;
        }
    }
}