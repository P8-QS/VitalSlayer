using System.Collections;
using UnityEngine;

public class Necromancer : Enemy
{
    [Header("Ranged Settings")]
    public RangedEnemyStats rangedStats;

    [Header("Fireball References")]
    public Transform fireballSpawnPoint;

    private float lastAttackTime;
    private Animator animator;
    private bool isAttacking = false;

    protected override void Start()
    {
        // Override normal enemy stats with ranged-specific stats if provided
        if (rangedStats != null)
        {
            enemyStats = rangedStats;
        }

        base.Start();
        animator = GetComponent<Animator>();
        lastAttackTime = -rangedStats.attackCooldown; // Allow immediate attack when first encountering player
    }

    protected new void FixedUpdate()
    {
        if (Vector3.Distance(playerTransform.position, startingPosition) < (enemyStats != null ? enemyStats.chaseLength : 5f))
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < (enemyStats != null ? enemyStats.triggerLength : 1f))
            {
                chasing = true;
            }

            if (chasing)
            {
                if (Time.time > lastAttackTime + rangedStats.attackCooldown && !isAttacking)
                {
                    CastFireball();
                }

                if (!collidingWithPlayer)
                {
                    // Try to keep some distance for ranged attacks
                    if (Vector3.Distance(transform.position, playerTransform.position) < rangedStats.retreatDistance)
                    {
                        // Move away from player if too close
                        Vector3 directionAwayFromPlayer = (transform.position - playerTransform.position).normalized * currentSpeed;
                        UpdateMotor(directionAwayFromPlayer);
                    }
                    else if (Vector3.Distance(transform.position, playerTransform.position) > rangedStats.chaseDistance)
                    {
                        // Move closer if too far
                        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized * currentSpeed;
                        UpdateMotor(directionToPlayer * 0.5f); // Move at half speed when approaching
                    }
                    else
                    {
                        UpdateMotor(Vector3.zero);
                    }
                }
            }
            else
            {
                UpdateMotor(startingPosition - transform.position);
            }
        }
        else
        {
            // Return to starting position if player is out of range
            UpdateMotor(startingPosition - transform.position);
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

            if (hits[i].tag == "Player")
            {
                collidingWithPlayer = true;
            }

            hits[i] = null;
        }
    }

    private void CastFireball()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        if (directionToPlayer.x > 0)
        {
            transform.localScale = new Vector3(initialSize.x, initialSize.y, initialSize.z);
        }
        else if (directionToPlayer.x < 0)
        {
            transform.localScale = new Vector3(-initialSize.x, initialSize.y, initialSize.z);
        }

        // Spawn and shoot fireball
        ShootFireball();

        isAttacking = false;
    }

    private void ShootFireball()
    {
        Vector3 spawnPosition = fireballSpawnPoint != null ?
            fireballSpawnPoint.position :
            transform.position + new Vector3(0, 0.1f, 0);

        GameObject fireball = Instantiate(rangedStats.projectilePrefab, spawnPosition, Quaternion.identity);

        Vector3 direction = (playerTransform.position - spawnPosition).normalized;

        FireballProjectile fireballComponent = fireball.AddComponent<FireballProjectile>();

        int minDamage = rangedStats.GetScaledRangedMinDamage(level);
        int maxDamage = rangedStats.GetScaledRangedMaxDamage(level);
        int fireballDamage = GameHelpers.CalculateDamage(minDamage, maxDamage, 0f, 0f);

        fireballComponent.Initialize(this, direction, rangedStats.projectileSpeed, fireballDamage,
            rangedStats.projectileLifetime, minDamage, maxDamage);
    }

    public int GetMinProjectileDamage()
    {
        return rangedStats.GetScaledRangedMinDamage(level);
    }

    public int GetMaxProjectileDamage()
    {
        return rangedStats.GetScaledRangedMaxDamage(level);
    }
}

public class FireballProjectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private int damage;
    private float lifetime;
    private float spawnTime;
    private float timeAlive;
    private bool hasHitTarget = false;
    private int minDamage;
    private int maxDamage;

    private Necromancer necromancer;
    private BoxCollider2D hitbox;
    private Animator animator;

    public void Initialize(Necromancer necromancer, Vector3 direction, float speed, int damage, float lifetime, int minDamage, int maxDamage)
    {
        this.necromancer = necromancer;
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
        this.lifetime = lifetime;
        this.spawnTime = Time.time;
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;

        animator = GetComponent<Animator>();
        hitbox = GetComponent<BoxCollider2D>();

        // Set the rotation to face the direction of travel
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        timeAlive = (Time.time - spawnTime) / lifetime;
        if (animator != null)
        {
            animator.SetFloat("timeAlive", timeAlive);
        }

        transform.position += direction * speed * Time.deltaTime;

        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHitTarget)
            return;

        // Check for collision with player or environment
        if (collision.tag == "Player")
        {
            Damage dmg = new Damage
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = 2.0f,
                isCritical = false,
                minPossibleDamage = minDamage,
                maxPossibleDamage = maxDamage,
            };

            collision.SendMessage("ReceiveDamage", dmg);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Blocking" || collision.gameObject.name == "Walls")
        {
            hasHitTarget = true;
            StartDestroySequence();
        }
    }

    private void StartDestroySequence()
    {
        if (hitbox != null)
            hitbox.enabled = false;

        speed = 0f;

        if (animator != null)
        {
            animator.SetTrigger("end");
            StartCoroutine(WaitForSpecificAnimationToComplete());
        }
    }

    private IEnumerator WaitForSpecificAnimationToComplete()
    {
        // Hard coded wait time for the animation to finish
        yield return new WaitForSeconds(0.333f);

        Destroy(gameObject);
    }
}