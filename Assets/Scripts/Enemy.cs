using UnityEngine;
using Effects;

public class Enemy : Mover
{
    protected BaseEnemyStats enemyStats;

    // Logic
    protected bool chasing;
    protected bool collidingWithPlayer;
    protected Transform playerTransform;
    protected Vector3 startingPosition;

    private bool _isPhantom;
    public bool isPhantom
    {
        set
        {
            if (value)
            {
                hitpoint = 1;
                maxHitpoint = 1;
            }
            _isPhantom = value;
        }
        get => _isPhantom;
    }

    // Hitbox
    public ContactFilter2D filter;
    public BoxCollider2D hitBox;
    public Collider2D[] hits = new Collider2D[10];

    protected override void Start()
    {
        if (enemyStats == null)
        {
            Debug.LogError("Enemy stats not assigned to " + gameObject.name);
            return;
        }

        if (stats == null)
        {
            stats = enemyStats;
        }

        base.Start();
        playerTransform = GameManager.Instance.player.transform;
        startingPosition = transform.position;
        hitBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    protected void SetEnemyStats(BaseEnemyStats newStats)
    {
        enemyStats = newStats;
        SetStats(newStats);
    }

    protected void FixedUpdate()
    {
        if (!playerTransform) return;

        float chaseDistance = enemyStats != null ? enemyStats.chaseLength : 5f;
        float triggerDistance = enemyStats != null ? enemyStats.triggerLength : 1f;

        // Is the player in range?
        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseDistance)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerDistance)
            {
                chasing = true;
            }

            if (chasing)
            {
                if (!collidingWithPlayer)
                {
                    UpdateMotor((playerTransform.position - transform.position).normalized * currentSpeed);
                }
            }
            else
            {
                UpdateMotor((startingPosition - transform.position) * currentSpeed);
            }
        }
        else
        {
            UpdateMotor((startingPosition - transform.position) * currentSpeed);
            chasing = false;
        }

        // Check for overlaps
        collidingWithPlayer = false;
        boxCollider.Overlap(filter, hits);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
            {
                continue;
            }

            if (hits[i].CompareTag("Player"))
            {
                collidingWithPlayer = true;
            }

            // Reset array
            hits[i] = null;
        }
    }

    protected override void Death()
    {
        if (!isPhantom)
        {
            int reward = enemyStats != null ? enemyStats.GetScaledXpReward(level) : 10;
            int xp = ExperienceManager.Instance.AddEnemy(level);
            GameSummaryManager.Instance.AddEnemy();
            FloatingTextManager.Instance.Show("+" + xp + " xp", 10, Color.magenta, transform.position, Vector3.up * 1,
                1.0f);
        }

        AudioClip sound = enemyStats != null ? enemyStats.deathSound : null;
        if (sound != null)
        {
            SoundFxManager.Instance.PlaySound(sound, 0.5f);
        }

        Destroy(gameObject);
    }
}