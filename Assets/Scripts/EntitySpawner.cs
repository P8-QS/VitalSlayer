using System.Collections.Generic;
using UnityEngine;
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

            if (availablePoints.Count == 0)
            {
                Debug.LogWarning($"No available spawn points for {typeof(T).Name} in room {roomIndex}");
                return;
            }

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
}