using System;
using System.Linq;
using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    private static FPSLimiter instance;
    private const double FallbackFPS = 60;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        var resolutions = Screen.resolutions;
        var maxFPS = resolutions.Max(res => res.refreshRateRatio.value);
        var targetFPS = Math.Max(FallbackFPS, maxFPS);


        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = (int)targetFPS;
        Debug.Log($"RefreshRateRatio: {maxFPS}");
        Debug.Log($"Set targetFrameRate (FPS): {targetFPS}");
    }
}