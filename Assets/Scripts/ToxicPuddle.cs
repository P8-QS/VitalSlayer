using System.Collections;
using Effects;
using UnityEngine;

public class ToxicPuddle : Collidable
{
    private AcidSlime acidSlime;

    public float duration = 5.0f;

    public int minDamage = 1;
    public int maxDamage = 1;
    public float damageTickRate = 0.5f;
    public float dotDuration = 3f;
    public float slowFactor = 0.5f;

    public Color puddleColor;

    private float creationTime;
    private SpriteRenderer spriteRenderer;
    private System.Random random = new System.Random();

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();

        creationTime = Time.time;

        ApplyPuddleColor();

        StartCoroutine(FadeOut());
    }

    protected override void Update()
    {
        base.Update();

        // Check if the puddle should be destroyed
        if (Time.time - creationTime >= duration)
        {
            Destroy(gameObject);
        }
    }

    public void ApplyPuddleColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = puddleColor;
            Debug.Log($"Applied puddle color: {puddleColor}");
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on ToxicPuddle!");
        }
    }

    protected override void OnCollide(Collider2D coll)
    {
        var slime = coll.GetComponent<AcidSlime>();

        // If the collision was an acid slime do nothing
        if (slime != null)
        {
            return;
        }

        Fighter target = coll.gameObject.GetComponent<Fighter>();
        if (target)
        {
            Damage damage = new Damage
            {
                damageAmount = GameHelpers.CalculateDamage(minDamage, maxDamage),
                origin = transform.position,
                pushForce = 0f, // No push force for puddle damage
                isCritical = false,
                minPossibleDamage = minDamage,
                maxPossibleDamage = maxDamage,
                useCustomColor = true,
                /*customColor = new Color(puddleColor.r, puddleColor.g, puddleColor.b, 1f) // Make fully opaque for text */
                customColor = Color.red // Use red for damage text
            };

            if (!ToxicPuddleEffect.PuddleImmunity)
            {
                slowFactor = ToxicPuddleEffect.SlowFactor;
                target.ApplyDamageOverTime(damage, dotDuration, damageTickRate);
                target.ApplySlowEffect(slowFactor, damageTickRate * 2);
            }
        }
    }


    private IEnumerator FadeOut()
    {
        // Initial alpha
        float alpha = puddleColor.a;

        // Wait until it's time to start fading
        yield return new WaitForSeconds(duration * 0.7f);

        // Calculate how long to fade
        float fadeTime = duration * 0.3f;
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeTime)
        {
            // Calculate the new alpha
            float newAlpha = Mathf.Lerp(alpha, 0f, elapsedTime / fadeTime);

            // Update the color
            if (spriteRenderer != null)
            {
                Color newColor = spriteRenderer.color;
                newColor.a = newAlpha;
                spriteRenderer.color = newColor;
            }

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}