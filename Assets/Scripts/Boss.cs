using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss Settings")]
    public RedBossStats redBossStats;

    private Transform[] fireballs;

    protected override void Start()
    {
        if (redBossStats != null)
        {
            enemyStats = redBossStats;
        }

        base.Start();

        // Initialize fireballs based on boss stats
        InitializeFireballs();
    }

    private void InitializeFireballs()
    {
        if (redBossStats == null || redBossStats.fireballPrefab == null)
        {
            Debug.LogError("Boss stats or fireball prefab not assigned!");
            return;
        }

        fireballs = new Transform[redBossStats.numFireballs];
        for (int i = 0; i < redBossStats.numFireballs; i++)
        {
            GameObject fireballObj = Instantiate(redBossStats.fireballPrefab, transform.position, Quaternion.identity);
            fireballs[i] = fireballObj.transform;
            fireballObj.transform.parent = transform;
        }
    }

    private void Update()
    {
        if (fireballs == null || fireballs.Length == 0)
            return;

        for (int i = 0; i < fireballs.Length; i++)
        {
            if (fireballs[i] != null)
            {
                float speed = (i < redBossStats.fireballSpeed.Length) ?
                    redBossStats.fireballSpeed[i] : redBossStats.fireballSpeed[0];

                fireballs[i].position = transform.position +
                    new Vector3(
                        -Mathf.Cos(Time.time * speed) * redBossStats.fireballDistance,
                        Mathf.Sin(Time.time * speed) * redBossStats.fireballDistance,
                        0
                    );
            }
        }
    }

    protected override void Death()
    {
        Destroy(gameObject);
        int reward = enemyStats.GetScaledXpReward(level);
        int xp = ExperienceManager.Instance.AddExperience(reward);
        GameSummaryManager.Instance.AddBoss();
        FloatingTextManager.Instance.Show("+" + xp + " xp", 10, Color.magenta, transform.position, Vector3.up * 1,
            1.0f);
        GameSummaryManager.Instance.Show();
    }
}