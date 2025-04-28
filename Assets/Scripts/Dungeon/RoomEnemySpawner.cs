using UnityEngine;
using Dungeon;

public class RoomEnemySpawner : MonoBehaviour
{
    private Room room;
    private bool hasSpawnedEnemies = false;

    [Header("Boss Room")]
    [HideInInspector] public bool isBossRoom = false;

    private void Awake()
    {
        room = GetComponent<Room>();
        if (room == null)
        {
            Debug.LogError("RoomEnemySpawner requires a Room component on the same GameObject", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        // Listen to player entering room events
        if (room != null)
        {
            // Use a trigger component to listen for player entry
            RoomTrigger trigger = GetComponentInChildren<RoomTrigger>();
            if (trigger != null)
            {
                trigger.OnPlayerEnterRoom += OnPlayerEnteredRoom;
            }
        }

        // Listen to fog visibility changes to spawn enemies when they become visible
        RoomFogController fogController = GetComponent<RoomFogController>();
        if (fogController != null)
        {
            fogController.OnFogVisibilityChanged += OnFogVisibilityChanged;
        }
        else
        {
            Debug.LogWarning("No RoomFogController found on room: " + gameObject.name);
        }
    }

    private void OnDisable()
    {
        // Clean up event subscriptions
        if (room != null)
        {
            RoomTrigger trigger = GetComponentInChildren<RoomTrigger>();
            if (trigger != null)
            {
                trigger.OnPlayerEnterRoom -= OnPlayerEnteredRoom;
            }
        }

        RoomFogController fogController = GetComponent<RoomFogController>();
        if (fogController != null)
        {
            fogController.OnFogVisibilityChanged -= OnFogVisibilityChanged;
        }
    }

    private void OnPlayerEnteredRoom()
    {
        if (!hasSpawnedEnemies)
        {
            SpawnEnemiesInRoom();
            hasSpawnedEnemies = true;
        }
    }

    private void OnFogVisibilityChanged(bool isFogActive)
    {
        // If fog is removed and enemies haven't been spawned yet, spawn them
        if (!isFogActive && !hasSpawnedEnemies)
        {
            SpawnEnemiesInRoom();
            hasSpawnedEnemies = true;
        }
    }

    private void SpawnEnemiesInRoom()
    {
        int roomIndex = EntitySpawner.Instance.rooms.IndexOf(gameObject);
        if (roomIndex == -1)
        {
            Debug.LogWarning($"Room {gameObject.name} not found in EntitySpawner's room list", this);
            return;
        }

        if (isBossRoom)
        {
            EntitySpawner.Instance.SpawnEnemies<Boss>(roomIndex, 1);
        }
        else
        {
            // Use the centralized settings from GameManager
            int minEnemies = GameManager.Instance.minEnemiesPerRoom;
            int maxEnemies = GameManager.Instance.maxEnemiesPerRoom;
            int phantomEnemies = GameManager.Instance.phantomEnemiesPerRoom;

            int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
            EntitySpawner.Instance.SpawnEnemies<Enemy>(roomIndex, enemyCount);

            // Spawn phantom enemies if configured
            if (phantomEnemies > 0)
            {
                EntitySpawner.Instance.SpawnEnemies<Enemy>(roomIndex, phantomEnemies, true);
            }
        }

        // Update room enemies list to track them
        room.FindEnemiesInRoom();
    }

    public void SetBossRoom(bool isBoss)
    {
        isBossRoom = isBoss;
    }
}
