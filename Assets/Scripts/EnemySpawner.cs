using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject acidSlimePrefab;

    public void SpawnPhantomEnemy(Vector3 position)
    {
        GameObject enemyGO = Instantiate(acidSlimePrefab, position, Quaternion.identity);
        Enemy enemy = enemyGO.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.isPhantom = true;
        }
    }
}