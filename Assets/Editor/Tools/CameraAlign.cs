using UnityEditor;
using UnityEngine;

public class CameraAlignTool : EditorWindow
{
    [MenuItem("Tools/Align Camera to Selected")]
    static void AlignCamera()
    {
        if (Selection.activeTransform != null && Camera.main != null)
        {
            Undo.RecordObject(Camera.main.transform, "Align Camera");
            Camera.main.transform.position = Selection.activeTransform.position;
            Camera.main.transform.rotation = Selection.activeTransform.rotation;
        }
    }
}