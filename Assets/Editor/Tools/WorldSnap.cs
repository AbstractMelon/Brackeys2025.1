using UnityEngine;
using UnityEditor;

public class WorldSnap : EditorWindow
{
    [MenuItem("Tools/World Snap")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(WorldSnap));
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Snap Selected Objects to Ground"))
        {
            SnapToGround();
        }
    }

    private void SnapToGround()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            RaycastHit hit;
            if (Physics.Raycast(obj.transform.position + new Vector3(0, obj.GetComponent<Renderer>().bounds.extents.y, 0), Vector3.down, out hit))
            {
                Undo.RecordObject(obj.transform, "Snap to Ground");
                Vector3 newPosition = obj.transform.position;
                newPosition.y = hit.point.y + obj.GetComponent<Renderer>().bounds.extents.y - 1f;
                obj.transform.position = newPosition;
            }
        }
    }
}

