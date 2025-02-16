using UnityEditor;
using UnityEngine;

public class TagSetupTool : EditorWindow
{
    private string newTagName = "NewTag";

    [MenuItem("Tools/Create New Tag")]
    public static void ShowWindow() => GetWindow<TagSetupTool>("Tag Creator");

    void OnGUI()
    {
        GUILayout.Label("Create New Tag:", EditorStyles.boldLabel);
        newTagName = EditorGUILayout.TextField("Tag Name:", newTagName);

        if (GUILayout.Button("Create Tag"))
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tags = tagManager.FindProperty("tags");

            for (int i = 0; i < tags.arraySize; i++)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == newTagName) return;
            }

            tags.InsertArrayElementAtIndex(tags.arraySize);
            tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = newTagName;
            tagManager.ApplyModifiedProperties();
        }
    }
}