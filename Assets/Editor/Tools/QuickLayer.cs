using UnityEditor;
using UnityEngine;

public class LayerSetupTool : EditorWindow
{
    private string newLayerName = "NewLayer";

    [MenuItem("Tools/Layer Setup")]
    public static void ShowWindow()
    {
        GetWindow<LayerSetupTool>("Layer Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create New Layer:", EditorStyles.boldLabel);
        newLayerName = EditorGUILayout.TextField("Layer Name:", newLayerName);
        
        if (GUILayout.Button("Create Layer"))
        {
            CreateNewLayer();
        }
    }

    private void CreateNewLayer()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        for (int i = 8; i < layers.arraySize; i++)
        {
            if (layers.GetArrayElementAtIndex(i).stringValue == "")
            {
                layers.GetArrayElementAtIndex(i).stringValue = newLayerName;
                tagManager.ApplyModifiedProperties();
                Debug.Log($"Created new layer: {newLayerName} at index {i}");
                return;
            }
        }
        
        Debug.LogError("No empty layers available!");
    }
}