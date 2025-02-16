using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class PhysicsVisualizer
{
    static PhysicsVisualizer()
    {
        SceneView.duringSceneGui += DrawPhysicsGizmos;
    }

    static void DrawPhysicsGizmos(SceneView sceneView)
    {
        foreach(Collider col in Object.FindObjectsByType<Collider>(FindObjectsSortMode.None))
        {
            Handles.color = GetColliderColor(col);
            DrawCollider(col);
        }
    }

    static Color GetColliderColor(Collider col)
    {
        return col.enabled ? 
            (col.isTrigger ? Color.cyan : new Color(1,0.5f,0)) : 
            new Color(0.5f,0.5f,0.5f,0.3f);
    }

    static void DrawCollider(Collider col)
    {
        if(col is BoxCollider box)
        {
            Vector3 size = Vector3.Scale(box.size, box.transform.lossyScale);
            Handles.DrawWireCube(box.transform.TransformPoint(box.center), size);
        }
        else if(col is SphereCollider sphere)
        {
            Handles.DrawWireDisc(
                sphere.transform.TransformPoint(sphere.center), 
                sphere.transform.up, 
                sphere.radius * sphere.transform.lossyScale.x
            );
        }
    }
}
