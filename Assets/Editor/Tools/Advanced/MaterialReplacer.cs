using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class AdvancedMaterialReplacer : EditorWindow
{
    private Regex materialPattern;
    private Material replacementMaterial;
    
    [MenuItem("Tools/Advanced/Batch Material Replacer")]
    static void Init() => GetWindow<AdvancedMaterialReplacer>();

    void OnGUI()
    {
        EditorGUILayout.LabelField("Regex Pattern:", EditorStyles.boldLabel);
        string pattern = EditorGUILayout.TextField("Material Name Pattern", materialPattern?.ToString());
        materialPattern = new Regex(pattern ?? "", RegexOptions.IgnoreCase);

        replacementMaterial = (Material)EditorGUILayout.ObjectField("Replacement Material", 
            replacementMaterial, typeof(Material), false);

        if(GUILayout.Button("Replace Across Project"))
        {
            ReplaceMaterials();
        }
    }

    void ReplaceMaterials()
    {
        int count = 0;
        foreach(Renderer renderer in FindAllComponents<Renderer>())
        {
            List<Material> mats = new List<Material>(renderer.sharedMaterials);
            bool modified = false;

            for(int i = 0; i < mats.Count; i++)
            {
                if(mats[i] && materialPattern.IsMatch(mats[i].name))
                {
                    Undo.RecordObject(renderer, "Material Replacement");
                    mats[i] = replacementMaterial;
                    modified = true;
                    count++;
                }
            }

            if(modified) renderer.sharedMaterials = mats.ToArray();
        }

        Debug.Log($"Replaced {count} materials");
    }

    List<T> FindAllComponents<T>() where T : Component
    {
        List<T> results = new List<T>();
        foreach(GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if(go.hideFlags == HideFlags.None && go.scene.IsValid())
            {
                results.AddRange(go.GetComponentsInChildren<T>());
            }
        }
        return results;
    }
}