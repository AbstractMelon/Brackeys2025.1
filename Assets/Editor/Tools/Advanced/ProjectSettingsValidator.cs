using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ProjectSettingsValidator : EditorWindow
{
    [MenuItem("Tools/Advanced/Project Settings Validator")]
    static void Init() => GetWindow<ProjectSettingsValidator>();

    void OnGUI()
    {
        if(GUILayout.Button("Check For Issues"))
        {
            ValidateLayers();
            ValidateTags();
            ValidateInputManager();
        }
    }

    void ValidateLayers()
    {
        var missingLayers = new List<string>();
        foreach(GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if(go.layer == 0 && go.tag != "Untagged")
            {
                missingLayers.Add(go.name);
            }
        }

        if(missingLayers.Count > 0)
        {
            Debug.LogError($"Missing layer assignments on: {string.Join(", ", missingLayers)}");
        }
    }

    void ValidateTags()
    {
        var missingTags = new List<string>();
        foreach(GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if(string.IsNullOrEmpty(go.tag) || go.tag == "Untagged")
            {
                missingTags.Add(go.name);
            }
        }

        if(missingTags.Count > 0)
        {
            Debug.LogError($"Missing tag assignments on: {string.Join(", ", missingTags)}");
        }
    }

    void ValidateInputManager()
    {
        var inputManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        var axes = inputManager.FindProperty("m_Axes");
        var duplicateAxes = new HashSet<string>();
        var seenAxes = new HashSet<string>();

        for (int i = 0; i < axes.arraySize; i++)
        {
            var axis = axes.GetArrayElementAtIndex(i);
            var name = axis.FindPropertyRelative("m_Name").stringValue;

            if (seenAxes.Contains(name))
            {
                duplicateAxes.Add(name);
            }
            else
            {
                seenAxes.Add(name);
            }
        }

        if(duplicateAxes.Count > 0)
        {
            Debug.LogError($"Duplicate input axes found: {string.Join(", ", duplicateAxes)}");
        }
    }
}

