using System.Collections.Generic;
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
            Debug.Log("Game scene loaded. Spawning entities.");

            var dungeonGenerator = FindFirstObjectByType<DungeonGenerator>();
            dungeonGenerator.GenerateDungeon();

            var rooms = dungeonGenerator.placedRooms.Select(room => room.GameObject).ToList();
            EntitySpawner.Instance.rooms = rooms;

            var world = GameObject.FindGameObjectWithTag("World");
            EntitySpawner.Instance.entityParent = world.transform;

            EntitySpawner.Instance.FillRoomsWithEntities(0, 2);


            GameSummaryManager.Instance.Reset();
        });
    }
}