using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class SetGradleVersion : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    
    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.Android) return;
        EditorPrefs.SetString("GradleDistribution", "https://services.gradle.org/distributions/gradle-8.11.1-all.zip");
        Debug.Log("Set Gradle distribution URL to 8.11.1");
    }
}
