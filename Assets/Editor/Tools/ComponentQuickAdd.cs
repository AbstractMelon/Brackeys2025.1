using UnityEditor;
using UnityEngine;

public class ComponentQuickAdd : EditorWindow
{
    private bool addRigidbody = false;
    private bool addCollider = false;

    [MenuItem("Tools/Quick Component Setup")]
    public static void ShowWindow()
    {
        GetWindow<ComponentQuickAdd>("Component Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Add Components to Selected:", EditorStyles.boldLabel);
        addRigidbody = EditorGUILayout.Toggle("Rigidbody", addRigidbody);
        addCollider = EditorGUILayout.Toggle("Box Collider", addCollider);

        if (GUILayout.Button("Apply Components"))
        {
            AddComponents();
        }
    }

    private void AddComponents()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (addRigidbody && !obj.GetComponent<Rigidbody>())
            {
                Undo.AddComponent<Rigidbody>(obj);
            }
            
            if (addCollider && !obj.GetComponent<BoxCollider>())
            {
                Undo.AddComponent<BoxCollider>(obj);
            }
        }
    }
}