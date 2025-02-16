using UnityEditor;
using UnityEngine;

public class MaterialPainter : EditorWindow
{
    private Material selectedMaterial;

    [MenuItem("Tools/Material Painter")]
    public static void ShowWindow() => GetWindow<MaterialPainter>("Material Painter");

    void OnGUI()
    {
        GUILayout.Label("Drag material to apply to selected objects:", EditorStyles.boldLabel);
        selectedMaterial = (Material)EditorGUILayout.ObjectField(selectedMaterial, typeof(Material), false);

        if (GUILayout.Button("Apply Material") && selectedMaterial != null)
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                if (obj.GetComponent<Renderer>())
                {
                    Undo.RecordObject(obj.GetComponent<Renderer>(), "Apply Material");
                    obj.GetComponent<Renderer>().sharedMaterial = selectedMaterial;
                }
            }
        }
    }
}