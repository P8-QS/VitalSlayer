using System.Linq;
using Effects;
using Managers;
using UnityEngine;

public class Enemy : Mover
{
    [Header("Drops")] [Tooltip("Health potion prefab that may drop when enemy dies")]
    public GameObject healthPotionPrefab;

    [Header("Phantom Effects")] [Tooltip("Dust animation prefab that plays when phantom enemies die")]
    public GameObject phantomDustPrefab;

    [HideInInspector] public BaseEnemyStats enemyStats;

    // Logic
    protected bool chasing;
    protected bool collidingWithPlayer;
    protected Transform playerTransform;
    protected Vector3 startingPosition;
    public GameObject room;
    private bool _isPhantom;

    public bool isPhantom
    {
        set => _isPhantom = value;
        get => _isPhantom;
    }

    // Hitbox
    public ContactFilter2D filter;
    public BoxCollider2D hitBox;
    [HideInInspector] public Collider2D[] hits = new Collider2D[10];

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


        // Scuffed LevelIndicatorEffect apply but works
        var hideLvlIndicator = MetricsManager.Instance.metrics.Values.SelectMany(m => m.Effects)
            .Any(e => e is LevelIndicatorEffect && e.Level == 0);


        if (EnemyUIManager.Instance != null && !hideLvlIndicator)
        {
            EnemyUIManager.Instance.CreateLevelIndicator(this);
        }

        // Scuffed CombatInfoEffect apply but works
        var hideHealthBar = MetricsManager.Instance.metrics.Values.SelectMany(m => m.Effects)
            .Any(e => e is CombatInfoEffect && e.Level != 1);
        if (hideHealthBar) Destroy(healthBar.gameObject);


        if (_isPhantom)
        {
            maxHitpoint = 1;
            hitpoint = maxHitpoint;
        }

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

        var roomBoundsCollider = room.GetComponentInChildren<BoxCollider2D>();

        Bounds bounds = roomBoundsCollider.bounds;

        // Is the player in range?
        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseDistance)
        {
            if (HasLineOfSight() &&
                Vector3.Distance(playerTransform.position, startingPosition) < enemyStats.triggerLength)
            {
                chasing = true;
            }

            if (chasing)
            {
                if (!collidingWithPlayer)
                {
                    Vector3 velocity = (playerTransform.position - transform.position).normalized * currentSpeed;
                    Vector3 nextPos = transform.position + velocity * Time.deltaTime;

                    if (bounds.Contains(nextPos))
                    {
                        UpdateMotor(velocity);
                    }
                    else
                    {
                        chasing = false;
                    }
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

            hits[i] = null;
        }
    }

    public bool HasLineOfSight()
    {
        Vector2 origin = transform.position;
        Vector2 target = playerTransform.position;
        Vector2 direction = (target - origin).normalized;
        float distance = Vector2.Distance(origin, target);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, LayerMask.GetMask("Default"));

        if (hit.collider != null)
        {
            // Ray hit a wall — no line of sight
            return false;
        }

        // No wall between enemy and player
        return true;
    }

    protected void SpawnPhantomDustEffect()
    {
        if (phantomDustPrefab == null)
        {
            Debug.LogWarning($"Phantom dust prefab not assigned to {gameObject.name}");
            return;
        }

        GameObject dustEffect = Instantiate(phantomDustPrefab, transform.position, Quaternion.identity);
        Animator animator = dustEffect.GetComponent<Animator>();

        float duration = 1f;

        if (animator != null)
        {
            var clips = animator.runtimeAnimatorController?.animationClips;
            var smokeClip = clips?.FirstOrDefault(c => c.name == "smoke");

            if (smokeClip != null)
                duration = smokeClip.length;
        }

        Destroy(dustEffect, duration);
    }

    protected override void Death()
    {
        if (isPhantom)
        {
            SpawnPhantomDustEffect();
        }
        else
        {
            int reward = enemyStats.GetScaledXpReward(level);
            int xp = ExperienceManager.Instance.AddExperience(reward);
            GameSummaryManager.Instance.AddEnemy();
            FloatingTextManager.Instance.Show("+" + xp + " xp", 10, Color.magenta, transform.position, Vector3.up * 1,
                1.0f);

            //Adjust chance of dropping health potion here:
            if (Random.value < 0.10f && healthPotionPrefab != null)
            {
                Instantiate(healthPotionPrefab, transform.position, Quaternion.identity);
            }
        }

        SoundFxManager.Instance.PlaySound(enemyStats.deathSound, 0.5f);

        Destroy(gameObject);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (EnemyUIManager.Instance != null)
        {
            EnemyUIManager.Instance.RemoveLevelIndicator(this);
        }
    }
}