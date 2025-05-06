using System;
using System.Linq;
using Dungeon;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject playerPrefab;
    public AudioClip gameMusic;

    public Player player;

    public int mapSize = 3;

    [Header("Enemy Spawning")]
    public int minEnemiesPerRoom = 1;
    public int maxEnemiesPerRoom = 3;
    public int phantomEnemiesPerRoom = 0;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;

            if (SceneManager.GetSceneAt(0).name == "TestingGrounds")
            {
                // get player from the scene
                player = FindFirstObjectByType<Player>();
            }


            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartRound()
    {
        SoundFxManager.Instance.PlayMusic(gameMusic, 0.05f, false, 2f);

        // Load the game scene with a transition
        SceneLoader.Instance.LoadSceneWithTransition("Game", () =>
        {
            Debug.Log("Game scene loaded. Generating dungeon.");

            var dungeonGenerator = FindFirstObjectByType<DungeonGenerator>();
            dungeonGenerator.roomCount = mapSize;
            dungeonGenerator.GenerateDungeon();

            var rooms = dungeonGenerator.PlacedRooms.Select(room => room.GameObject).ToList();
            EntitySpawner.Instance.rooms = rooms;

            var world = GameObject.FindGameObjectWithTag("World");
            EntitySpawner.Instance.entityParent = world.transform;

            // Only spawn the player in the first room
            int playerRoomIndex = 0;
            var player = EntitySpawner.Instance.SpawnPlayer(playerRoomIndex);
            EntitySpawner.Instance.SpawnFairy();
            GameManager.Instance.player = player;
            PerksManager.Instance.ApplyPerksToPlayer();

            // Add RoomEnemySpawner to all rooms if they don't already have it
            foreach (var room in rooms)
            {
                if (room.GetComponent<RoomEnemySpawner>() == null)
                {
                    room.AddComponent<RoomEnemySpawner>();
                }
            }

            var bossRoom = rooms[rooms.Count - 1].GetComponent<RoomEnemySpawner>();
            if (bossRoom != null)
            {
                bossRoom.isBossRoom = true;
            }

            GameSummaryManager.Instance.roundStartTime = DateTime.UtcNow;
            GameSummaryManager.Instance.Reset();
            MinimapManager.Instance.VisitedRooms.Clear();
        });
    }
}