using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    private const double FallbackFPS = 60;

    private void Start()
    {
        var resolutions = Screen.resolutions;
        var maxFPS = resolutions.Max(res => res.refreshRateRatio.value);
        var targetFPS = Math.Max(FallbackFPS, maxFPS);
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = (int)targetFPS;
        Debug.Log($"RefreshRateRatio: {maxFPS}");
        Debug.Log($"Set targetFrameRate (FPS): {targetFPS}");
        EditorPrefs.SetString("GradleDistribution", "https://services.gradle.org/distributions/gradle-8.11.1-all.zip");
    }
}
