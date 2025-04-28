using UnityEngine;

[CreateAssetMenu(fileName = "NewBossEnemy", menuName = "Enemy Stats/Boss Enemy")]
public class BaseBossStats : BaseEnemyStats
{
    // Victory rewards
    public bool grantsAchievement = false;
    public string achievementId = "";
}