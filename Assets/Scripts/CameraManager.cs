using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Room")) return;
        var cam = other.GetComponentInChildren<CinemachineCamera>();
        if (cam == null) return;
        SetActiveCamera(cam);
        cam.Follow = transform;
    }

    private void SetActiveCamera(CinemachineCamera activeCam)
    {
        var cams = Resources.FindObjectsOfTypeAll<CinemachineCamera>();
        foreach (var cam in cams)
        {
            cam.Priority = 0;
        }

        activeCam.Priority = 10;
    }
}