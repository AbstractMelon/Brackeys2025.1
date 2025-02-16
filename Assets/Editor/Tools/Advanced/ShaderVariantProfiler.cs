using UnityEditor;
using UnityEngine;
using System.Linq;

public class ShaderVariantProfiler : EditorWindow
{
    [MenuItem("Tools/Advanced/Shader Variant Analyzer")]
    static void Init() => GetWindow<ShaderVariantProfiler>();

    private Vector2 scrollPos;
    private Shader[] allShaders;

    void OnEnable() => allShaders = Resources.FindObjectsOfTypeAll<Shader>();

    void OnGUI()
    {
        EditorGUILayout.LabelField("Shader Variant Report", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach(Shader s in allShaders.OrderByDescending(s => s.passCount))
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(s, typeof(Shader), false);
            EditorGUILayout.LabelField($"{s.passCount} passes | {GetVariantCount(s)} variants");
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    int GetVariantCount(Shader shader)
    {
        return GetShaderVariantCount(shader, true) + 
               GetShaderVariantCount(shader, false);
    }

    int GetShaderVariantCount(Shader shader, bool isLocal)
    {
        // var variants = shader.GetShaderKeywords();
        // variants = variants.Where(v => (isLocal && v.StartsWith("_LOCAL_")) || (!isLocal && !v.StartsWith("_LOCAL_"))).ToArray();
        // return variants.Length;
        return 0;
    }
}


