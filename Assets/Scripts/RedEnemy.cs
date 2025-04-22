using UnityEngine;

public class RedEnemy : Enemy
{
    public BaseEnemyStats redEnemyStats;

    protected override void Start()
    {
        if (redEnemyStats == null)
        {
            Debug.LogError("RedEnemyStats not assigned to " + gameObject.name);
            return;
        }

        enemyStats = redEnemyStats;

        base.Start();
    }
}