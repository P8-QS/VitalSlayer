using UnityEngine;

public class AcidSlime : Enemy
{
    [Header("Acid Properties")]
    public Color acidColor = Color.green;

    [Header("Meltdown Properties")]
    public GameObject toxicPuddlePrefab;
    public float puddleDuration = 5.0f;
    public int puddleDamage = 1;
    public float puddleSlowFactor = 0.5f;

    // Reference to the acid hitbox component
    private AcidSlimeHitbox acidHitbox;

    protected override void Start()
    {
        base.Start();

        // Find the hitbox and get the AcidSlimeHitbox component
        acidHitbox = hitBox.GetComponent<AcidSlimeHitbox>();

        if (acidHitbox == null)
        {
            Debug.LogError("AcidSlimeHitbox component not found on the hitbox. Make sure to replace EnemyHitbox with AcidSlimeHitbox on the hitbox GameObject.");
        }

        // You might want to add a visual indicator for the acid slime
        // For example, you could tint the sprite green
        GetComponent<SpriteRenderer>().color = acidColor;
    }

    // Override the Death method to spawn a toxic puddle
    protected override void Death()
    {
        // Spawn the toxic puddle at the position of the slime
        if (toxicPuddlePrefab != null)
        {
            GameObject puddle = Instantiate(toxicPuddlePrefab, transform.position, Quaternion.identity);
            ToxicPuddle puddleComponent = puddle.GetComponent<ToxicPuddle>();

            if (puddleComponent != null)
            {
                puddleComponent.duration = puddleDuration;
                puddleComponent.damage = puddleDamage;
                puddleComponent.slowFactor = puddleSlowFactor;
                puddleComponent.puddleColor = new Color(acidColor.r, acidColor.g, acidColor.b, 0.6f);
            }
            else
            {
                // If no puddle component, destroy after duration
                Destroy(puddle, puddleDuration);
            }
        }

        // Call the base Death method to handle XP and destroy this object
        base.Death();
    }
}