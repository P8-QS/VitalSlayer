using UnityEngine;

public class AcidSlime : Enemy
{
    [Header("Acid Settings")]
    [SerializeField] private AcidSlimeEnemyStats acidSlimeStats;

    [Header("Toxic Puddle")]
    public GameObject toxicPuddlePrefab;

    private AcidSlimeHitbox acidHitbox;
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        if (acidSlimeStats != null)
        {
            SetEnemyStats(acidSlimeStats);
        }

        base.Start();

        acidHitbox = hitBox.GetComponent<AcidSlimeHitbox>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (acidHitbox == null)
        {
            Debug.LogError("AcidSlimeHitbox component not found on the hitbox. Make sure to replace EnemyHitbox with AcidSlimeHitbox on the hitbox GameObject.");
        }
        else
        {
            ConfigureHitbox();
        }

        ApplyColorBasedOnLevel();
    }

    private void ApplyColorBasedOnLevel()
    {
        if (spriteRenderer == null || acidSlimeStats == null) return;

        Color slimeColor = acidSlimeStats.GetColorForLevel(level);

        spriteRenderer.color = slimeColor;

        Debug.Log($"Applied level {level} color to {gameObject.name}: {slimeColor}");
    }

    private void ConfigureHitbox()
    {
        if (acidHitbox != null && acidSlimeStats != null)
        {
            acidHitbox.acidMinDamage = acidSlimeStats.GetScaledAcidMinDamage(level);
            acidHitbox.acidMaxDamage = acidSlimeStats.GetScaledAcidMaxDamage(level);
            acidHitbox.acidDuration = acidSlimeStats.acidDuration;
            acidHitbox.acidTickRate = acidSlimeStats.acidTickRate;

            acidHitbox.acidColor = acidSlimeStats.GetColorForLevel(level);
        }
    }

    protected override void Death()
    {
        if (toxicPuddlePrefab == null)
        {
            Debug.LogError("Toxic Puddle prefab is not assigned in the AcidSlime script.");
            return;
        }

        if (isPhantom)
        {
            base.Death();
            return;
        }

        GameObject puddle = Instantiate(toxicPuddlePrefab, transform.position, Quaternion.identity);
        ToxicPuddle puddleComponent = puddle.GetComponent<ToxicPuddle>();

        if (puddleComponent != null && acidSlimeStats != null)
        {
            puddleComponent.duration = acidSlimeStats.puddleDuration;
            puddleComponent.minDamage = acidSlimeStats.GetScaledAcidMinDamage(level);
            puddleComponent.maxDamage = acidSlimeStats.GetScaledAcidMaxDamage(level);
            puddleComponent.slowFactor = acidSlimeStats.puddleSlowFactor;

            Color levelColor = acidSlimeStats.GetColorForLevel(level);
            puddleComponent.puddleColor = new Color(levelColor.r, levelColor.g, levelColor.b, 0.6f);
        }
        else
        {
            Destroy(puddle, acidSlimeStats != null ? acidSlimeStats.puddleDuration : 5f);
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