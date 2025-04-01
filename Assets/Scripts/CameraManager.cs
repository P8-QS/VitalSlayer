using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CinemachineCamera newCam = other.GetComponentInChildren<CinemachineCamera>();
            if (newCam != null)
            {
                SetActiveCamera(newCam);
            }
        }
    }

    private void SetActiveCamera(CinemachineCamera activeCam)
    {
        CinemachineCamera[] cams = Resources.FindObjectsOfTypeAll<CinemachineCamera>();
        foreach (var cam in cams)
        {
            cam.Priority = 0;
        }

        activeCam.Priority = 10;
    }
}