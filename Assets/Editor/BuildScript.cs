using UnityEditor;
using UnityEngine;

// Headless build entry points, invoked via:
//   Unity -batchmode -quit -executeMethod BuildScript.BuildAndroidGameplay
public static class BuildScript
{
    // Android APK containing only the GamePlay scene.
    public static void BuildAndroidGameplay()
    {
        var options = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/GamePlay.unity" },
            locationPathName = "Builds/gameplay.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None,
        };

        var report = BuildPipeline.BuildPlayer(options);
        var summary = report.summary;
        Debug.Log($"[BuildScript] result={summary.result} size={summary.totalSize} " +
                  $"errors={summary.totalErrors} warnings={summary.totalWarnings} output={summary.outputPath}");

        if (summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            EditorApplication.Exit(1);
    }
}
