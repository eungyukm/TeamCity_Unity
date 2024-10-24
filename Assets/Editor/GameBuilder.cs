using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class GameBuilder
{
        public enum BuildType
    {
        Development,
        DevelopmentScriptsOnly,
        Release,
        ReleaseScriptsOnly,
    }

    private const string gameName = "My Game Name";
    private static string buildNumber = "1";
    private static string buildDate;

    [MenuItem(gameName + "/Build Development Build")]
    public static void DevelopmentBuild()
    {
        buildNumber = GetBuildVersionFromCommandLine();
        BuildPlayer(BuildType.Development, buildNumber);
    }
    
    [MenuItem(gameName + "/Build Release Build")]
    public static void ReleaseBuild()
    {
        buildNumber = GetBuildVersionFromCommandLine();
        BuildPlayer(BuildType.Development, buildNumber);
    }
    
    [MenuItem(gameName + "/Build Debug Scripts Only Build")]
    public static void DebugScriptsOnlyBuild()
    {
        buildNumber = GetBuildVersionFromCommandLine();
        BuildPlayer(BuildType.Development, buildNumber);
    }
    
    [MenuItem(gameName + "/Build Release Scripts Only Build")]
    public static void ReleaseScriptsOnlyBuild()
    {
        buildNumber = GetBuildVersionFromCommandLine();
        BuildPlayer(BuildType.Development, buildNumber);
    }

    private static void BuildPlayer(BuildType type, string buildVersion)
    {
        Debug.Log("Building player: " + type);
        var debug = type is BuildType.Development or BuildType.DevelopmentScriptsOnly ? true : false;
        EditorUserBuildSettings.development = debug;
        EditorUserBuildSettings.allowDebugging = debug;
        EditorUserBuildSettings.connectProfiler = debug;
        
        var buildFolderParent = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
        Debug.Log("project root: " + buildFolderParent);
        var sourceDirectory = buildFolderParent + "/Builds";
        if (debug)
            sourceDirectory += "/Debug";
        else
        {
            sourceDirectory += "/Release";
        }

        var fullBuildPathWithFileExt = sourceDirectory + "/" + PlayerSettings.productName +".exe";
       
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GenerateSceneList(),
            locationPathName = fullBuildPathWithFileExt,
            target = BuildTarget.StandaloneWindows64,
            options = GetBuildType(type),
        };
       
        Debug.Log("Building Player to: " + sourceDirectory);
        try
        {
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                // convert build size to Mb 
                Debug.Log("Build succeeded: " + (summary.totalSize * 1024^2) + " Mb");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }

            Debug.Log("Build output: " + report.summary.outputPath);
        }
        catch
        {
            Debug.LogError("Error building player");
            return;
        }
    }
    
    private static string GetBuildVersionFromCommandLine()
    {
        var args = System.Environment.GetCommandLineArgs();
        Debug.Log(args);
        return Guid.NewGuid().ToString();
    }
    
    private static string[] GenerateSceneList()
    {
        var sceneNames = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            sceneNames.Add(scene.path);
        }

        return sceneNames.ToArray();
    }

    /// <summary>
    /// Generates BuildOptions from our BuildType
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static BuildOptions GetBuildType(BuildType type)
    {
        var options = new BuildOptions();
        switch (type)
        {
            case BuildType.Development:
            {
                options = BuildOptions.Development | BuildOptions.AllowDebugging;
                break;
            }
            case BuildType.DevelopmentScriptsOnly:
            {
                options = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.BuildScriptsOnly;
                break;
            }
            case BuildType.Release:
            {
                options = BuildOptions.None;
                break;
            }
            case BuildType.ReleaseScriptsOnly:
            {
                options = BuildOptions.BuildScriptsOnly;
                break;
            }
        }

        return options;
    }
}