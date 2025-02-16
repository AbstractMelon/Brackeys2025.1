using UnityEditor;
using UnityEngine;

public class TransformResetter : Editor
{
    [MenuItem("Tools/Reset Transform %#r")]
    static void ResetTransform()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Reset Transform");
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }
    }
}