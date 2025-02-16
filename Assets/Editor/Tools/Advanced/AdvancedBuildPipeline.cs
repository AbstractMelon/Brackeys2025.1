using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using UnityEditor.Build.Reporting;
using System.Linq;
using System.Collections.Generic;
using System;

public class AdvancedBuildPipeline : EditorWindow
{
    private List<BuildTarget> buildTargets = new List<BuildTarget>();
    private bool runAfterBuild = true;
    private const string BuildSetSaveKey = "BuildSetSaveKey";

    [MenuItem("Tools/Advanced/Build Pipeline")]
    static void Init() => GetWindow<AdvancedBuildPipeline>();

    void OnGUI()
    {
        GUILayout.Label("Platforms", EditorStyles.boldLabel);
        for (int i = 0; i < buildTargets.Count; i++)
        {
            GUILayout.BeginHorizontal();
            BuildTarget newTarget = (BuildTarget)EditorGUILayout.EnumPopup(buildTargets[i]);
            if (newTarget != buildTargets[i])
            {
                buildTargets[i] = newTarget;
            }

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                buildTargets.RemoveAt(i);
                i--;
                GUILayout.EndHorizontal();
                continue;
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Platform"))
        {
            buildTargets.Add(BuildTarget.StandaloneWindows64);
        }

        runAfterBuild = EditorGUILayout.Toggle("Run After Build", runAfterBuild);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Build and Deploy"))
        {
            PerformBuild();
        }
        if (GUILayout.Button("Save Build Set"))
        {
            SaveBuildSet();
        }
        if (GUILayout.Button("Load Build Set"))
        {
            LoadBuildSet();
        }
        GUILayout.EndHorizontal();
    }

    void PerformBuild()
    {
        string path = EditorUtility.SaveFolderPanel("Choose Build Location", "", "");
        if (string.IsNullOrEmpty(path)) return;

        foreach (BuildTarget target in buildTargets.ToList())
        {
            string targetPath = Path.Combine(path, target.ToString());
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                target = target,
                locationPathName = Path.Combine(targetPath, Application.productName)
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result == BuildResult.Succeeded && runAfterBuild)
            {
                Process.Start(new ProcessStartInfo(options.locationPathName) { UseShellExecute = true });
            }
        }
    }

    void SaveBuildSet()
    {
        string buildSet = string.Join(",", buildTargets.Select(t => t.ToString()).ToArray());
        EditorPrefs.SetString(BuildSetSaveKey, buildSet);
    }

    void LoadBuildSet()
    {
        string buildSet = EditorPrefs.GetString(BuildSetSaveKey, "");
        if (!string.IsNullOrEmpty(buildSet))
        {
            buildTargets = buildSet.Split(',').Select(s => (BuildTarget)Enum.Parse(typeof(BuildTarget), s)).ToList();
        }
    }
}

