using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraManager : MonoBehaviour
{
    private Tilemap currentTilemap;
    private CinemachineCamera currentCamera;
    private float lastSwitchTime = 0f;
    private float switchCooldown = 0.3f;

    private void Update()
    {
        Vector3 playerPosition = transform.position;

        Tilemap playerTilemap = GetTilemapAtPosition(playerPosition);

        // If we found a tilemap and it's different from the current one
        if (playerTilemap != null && playerTilemap != currentTilemap)
        {
            // Check cooldown
            if (Time.time - lastSwitchTime < switchCooldown)
                return;

            // Get the room parent
            Transform roomTransform = playerTilemap.transform.parent;
            if (roomTransform != null)
            {
                // Find the camera in this room
                CinemachineCamera roomCamera = roomTransform.GetComponentInChildren<CinemachineCamera>();
                if (roomCamera != null && roomCamera != currentCamera)
                {
                    // Switch camera
                    currentCamera = roomCamera;
                    currentTilemap = playerTilemap;
                    SetActiveCamera(roomCamera);
                    roomCamera.Follow = transform;
                    lastSwitchTime = Time.time;
                }
            }
        }
    }

    private Tilemap GetTilemapAtPosition(Vector3 position)
    {
        Tilemap[] tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);

        foreach (Tilemap tilemap in tilemaps)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(position);

            if (tilemap.HasTile(cellPosition))
            {
                return tilemap;
            }
        }

        return null;
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