using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MissingComponentFinder : EditorWindow
{
    [MenuItem("Tools/Find Missing Components")]
    static void FindMissing()
    {
        List<GameObject> problematicObjects = new List<GameObject>();
        
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            Component[] components = go.GetComponents<Component>();
            foreach (Component c in components)
            {
                if (c == null)
                {
                    problematicObjects.Add(go);
                    break;
                }
            }
        }

        Selection.objects = problematicObjects.ToArray();
        Debug.Log($"Found {problematicObjects.Count} objects with missing components");
    }
}

