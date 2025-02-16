using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Linq;
using System.Collections.Generic;

public class ExecutionOrderVisualizer : EditorWindow
{
    private Vector2 scrollPos;
    
    [MenuItem("Tools/Advanced/Execution Order Graph")]
    static void Init() => GetWindow<ExecutionOrderVisualizer>();

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        var orders = new List<MonoScript>();
        foreach(MonoScript script in MonoImporter.GetAllRuntimeMonoScripts())
        {
            System.Type type = script.GetClass();
            if(type != null && typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                orders.Add(script);
            }
        }

        var sorted = orders.OrderBy(s => MonoImporter.GetExecutionOrder(s)).ToList();
        
        foreach(MonoScript script in sorted)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(script.name, GUILayout.Width(200));
            EditorGUILayout.IntField(MonoImporter.GetExecutionOrder(script));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }
}