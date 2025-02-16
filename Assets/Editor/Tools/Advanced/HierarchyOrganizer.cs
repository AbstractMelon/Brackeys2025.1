using UnityEditor;
using UnityEngine;

public class HierarchyOrganizer : EditorWindow
{
    private string sectionName = "SECTION";
    
    [MenuItem("Tools/Advanced/Hierarchy Section %#h")]
    static void Init() => GetWindow<HierarchyOrganizer>("Hierarchy Tools");

    void OnGUI()
    {
        sectionName = EditorGUILayout.TextField("Section Header", sectionName);

        if(GUILayout.Button("Create New Section"))
        {
            CreateHierarchySection();
        }

        if(GUILayout.Button("Sort Children Alphabetically"))
        {
            SortSelectedChildren();
        }
    }

    void CreateHierarchySection()
    {
        GameObject section = new GameObject($"----- {sectionName.ToUpper()} -----");
        Undo.RegisterCreatedObjectUndo(section, "Create Section");
        section.transform.SetSiblingIndex(Selection.activeTransform ? 
            Selection.activeTransform.GetSiblingIndex() + 1 : 0);
    }

    static void SortSelectedChildren()
    {
        Transform parent = Selection.activeTransform;
        if(!parent) return;

        Undo.RecordObject(parent, "Sort Children");
        for(int i = 0; i < parent.childCount; i++)
        {
            for(int j = 0; j < parent.childCount; j++)
            {
                if(string.Compare(parent.GetChild(j).name, parent.GetChild(i).name) < 0)
                {
                    parent.GetChild(j).SetSiblingIndex(i);
                }
            }
        }
    }
}

