using UnityEngine;

[CreateAssetMenu(fileName = "NewRedBossEnemy", menuName = "Enemy Stats/Red Boss Enemy")]
public class RedBossStats : BaseBossStats
{
    [Header("Boss-specific Settings")]
    public float[] fireballSpeed = { 2.5f, -2.5f };
    public float fireballDistance = 0.25f;
    public int numFireballs = 2;
    public GameObject fireballPrefab;
}