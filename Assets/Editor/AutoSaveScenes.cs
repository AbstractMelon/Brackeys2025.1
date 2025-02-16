using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoSaver : MonoBehaviour
{
    private static float saveInterval = 60; // 1 minute
    private static double nextSaveTime;

    static AutoSaver()
    {
        EditorApplication.update += Update;
        nextSaveTime = EditorApplication.timeSinceStartup + saveInterval;
    }

    private static void Update()
    {
        if (EditorApplication.timeSinceStartup >= nextSaveTime && !EditorApplication.isPlaying)
        {
            SaveAll();
            nextSaveTime = EditorApplication.timeSinceStartup + saveInterval;
        }
    }

    private static void SaveAll()
    {
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        Debug.Log("Auto-saved scene and assets at " + System.DateTime.Now);
    }
}