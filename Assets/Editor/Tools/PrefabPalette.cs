using UnityEditor;
using UnityEngine;

public class PrefabPalette : EditorWindow
{
    private GameObject[] prefabs;
    private Vector2 scrollPosition;
    private int selectedIndex = 0;

    [MenuItem("Tools/Prefab Palette")]
    public static void ShowWindow()
    {
        GetWindow<PrefabPalette>("Prefab Palette");
    }

    private void OnGUI()
    {
        GUILayout.Label("Drag Prefabs Here:", EditorStyles.boldLabel);
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty("prefabs");
        EditorGUILayout.PropertyField(prop, true);
        so.ApplyModifiedProperties();

        if (prefabs != null && prefabs.Length > 0)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            selectedIndex = GUILayout.SelectionGrid(selectedIndex, GetPreviewTextures(), 4);
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Place in Scene"))
            {
                PlacePrefab();
            }
        }
    }

    private void PlacePrefab()
    {
        if (prefabs[selectedIndex] != null)
        {
            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[selectedIndex]);
            Undo.RegisterCreatedObjectUndo(newObj, "Create " + newObj.name);
            newObj.transform.position = Vector3.zero;
            Selection.activeObject = newObj;
        }
    }

    private Texture[] GetPreviewTextures()
    {
        Texture[] textures = new Texture[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            textures[i] = AssetPreview.GetAssetPreview(prefabs[i]);
        }
        return textures;
    }
}