using UnityEngine;

public class AcidSlime : Enemy
{
    [Header("Acid Settings")]
    [SerializeField] private AcidSlimeEnemyStats acidSlimeStats;

    [Header("Toxic Puddle")]
    public GameObject toxicPuddlePrefab;

    private AcidSlimeHitbox acidHitbox;

    protected override void Start()
    {
        if (acidSlimeStats != null)
        {
            SetEnemyStats(acidSlimeStats);
        }

        base.Start();

        acidHitbox = hitBox.GetComponent<AcidSlimeHitbox>();

        if (acidHitbox == null)
        {
            Debug.LogError("AcidSlimeHitbox component not found on the hitbox. Make sure to replace EnemyHitbox with AcidSlimeHitbox on the hitbox GameObject.");
        }
        else
        {
            ConfigureHitbox();
        }

        GetComponent<SpriteRenderer>().color = acidSlimeStats.acidColor;
    }

    private void ConfigureHitbox()
    {
        if (acidHitbox != null && acidSlimeStats != null)
        {
            acidHitbox.acidMinDamage = acidSlimeStats.GetScaledAcidMinDamage(level);
            acidHitbox.acidMaxDamage = acidSlimeStats.GetScaledAcidMaxDamage(level);
            acidHitbox.acidDuration = acidSlimeStats.acidDuration;
            acidHitbox.acidTickRate = acidSlimeStats.acidTickRate;
            acidHitbox.acidColor = acidSlimeStats.acidColor;
        }
    }

    // Override the Death method to spawn a toxic puddle
    protected override void Death()
    {
        // Spawn the toxic puddle at the position of the slime
        if (toxicPuddlePrefab != null)
        {
            GameObject puddle = Instantiate(toxicPuddlePrefab, transform.position, Quaternion.identity);
            ToxicPuddle puddleComponent = puddle.GetComponent<ToxicPuddle>();

            if (puddleComponent != null && acidSlimeStats != null)
            {
                puddleComponent.duration = acidSlimeStats.puddleDuration;
                puddleComponent.minDamage = acidSlimeStats.GetScaledAcidMinDamage(level);
                puddleComponent.maxDamage = acidSlimeStats.GetScaledAcidMaxDamage(level);
                puddleComponent.slowFactor = acidSlimeStats.puddleSlowFactor;
                puddleComponent.puddleColor = new Color(acidSlimeStats.acidColor.r, acidSlimeStats.acidColor.g, acidSlimeStats.acidColor.b, 0.6f);
            }
            else
            {
                Destroy(puddle, acidSlimeStats != null ? acidSlimeStats.puddleDuration : 5f);
            }
        }

        base.Death();
    }

    private void OnValidate()
    {
        if (acidSlimeStats != null)
        {
            SetStats(acidSlimeStats);
        }
    }
}