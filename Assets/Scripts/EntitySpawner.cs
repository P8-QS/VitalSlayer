using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public class EntitySpawner : MonoBehaviour
{
    public static EntitySpawner Instance;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<GameObject> enemyPrefabs;
    public List<GameObject> bossPrefabs;
    public List<GameObject> playerPrefabs;

    public Transform entityParent;

    public int maxEnemiesPerRoom = 2;
    public int minEnemiesPerRoom = 1;
    public int phantomEnemyCount = 0;


    public List<GameObject> rooms;
    private Dictionary<int, HashSet<Vector3>> _usedSpawnPoints = new();


    private List<Vector3> GetEntitySpawnPoints(int roomIndex, string pointTag)
    {
        List<Vector3> spawnPoints = new List<Vector3>();

        GameObject room = rooms[roomIndex];

        var points = room.GetComponentsInChildren<Transform>();

        foreach (var point in points)
        {
            if (point.CompareTag(pointTag))
            {
                spawnPoints.Add(point.position);
            }
        }

        return spawnPoints;
    }

    public void SpawnEnemies<T>(int roomIndex, int count, bool isPhantomEnemy = false) where T : Enemy
    {
        var allSpawnPoints = GetEntitySpawnPoints(roomIndex, "EnemySpawn");

        if (!_usedSpawnPoints.ContainsKey(roomIndex))
        {
            _usedSpawnPoints[roomIndex] = new HashSet<Vector3>();
        }

        var usedPoints = _usedSpawnPoints[roomIndex];
        var prefabs = typeof(T) switch
        {
            { } t when t == typeof(Enemy) => enemyPrefabs,
            { } t when t == typeof(Boss) => bossPrefabs,
            _ => null
        };

        for (var i = 0; i < count; i++)
        {
            // If all points have been used, reset
            if (usedPoints.Count >= allSpawnPoints.Count)
            {
                usedPoints.Clear();
            }

            // Get list of unused points
            var availablePoints = new List<Vector3>(allSpawnPoints);
            availablePoints.RemoveAll(p => usedPoints.Contains(p));

            var spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];
            usedPoints.Add(spawnPoint);

            var randomIndex = Random.Range(0, prefabs.Count);
            var prefab = prefabs[randomIndex];

            var enemy = SpawnEntity<T>(prefab, spawnPoint);
            enemy.isPhantom = isPhantomEnemy;
        }
    }


    public Player SpawnPlayer(int roomIndex)
    {
        var spawnPoints = GetEntitySpawnPoints(roomIndex, "PlayerSpawn");

        var randomIndex = Random.Range(0, playerPrefabs.Count);
        var prefab = playerPrefabs[randomIndex];
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        return SpawnEntity<Player>(prefab, spawnPoint);
    }

    private T SpawnEntity<T>(GameObject prefab, Vector3 spawnPoint)
        where T : Mover
    {
        var entityInstance = Instantiate(prefab, spawnPoint, Quaternion.identity);

        var entity = entityInstance.GetComponent<T>();

        entityInstance.transform.SetParent(entityParent, true);

        return entity;
    }

    /// <summary>
    /// Fills all rooms with enemies and a player. Should only be called once at the start of the game.
    /// </summary>
    /// <param name="playerRoomIndex"></param>
    /// <param name="boosRoomIndex"></param>
    public void FillRoomsWithEntities(int playerRoomIndex, int boosRoomIndex)
    {
        // Clear used spawn points for all rooms
        _usedSpawnPoints.Clear();

        // Spawn enemies and bosses in each room
        for (var i = 0; i < rooms.Count; i++)
        {
            if (i == playerRoomIndex)
            {
                var player = SpawnPlayer(playerRoomIndex);
                GameManager.Instance.player = player;
            }

            if (i == boosRoomIndex)
            {
                SpawnEnemies<Boss>(i, 1);
                continue; // Don't spawn other enemies for this room
            }

            var enemyCount = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
            SpawnEnemies<Enemy>(i, enemyCount);
            
            // Spawn phantom enemies
            SpawnEnemies<Enemy>(i, playerRoomIndex, true);
            
        }
    }
}