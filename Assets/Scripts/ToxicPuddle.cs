using System.Collections;
using UnityEngine;

public class ToxicPuddle : Collidable
{
    public float duration = 5.0f;
    public int damage = 1;
    public float damageTickRate = 0.5f;
    public float slowFactor = 0.5f;
    public Color puddleColor = new Color(0.2f, 0.8f, 0.2f, 0.6f);

    private float creationTime;
    private SpriteRenderer spriteRenderer;
    private bool isApplyingDamage = false;

    protected override void Start()
    {
        base.Start();
        creationTime = Time.time;

        // Get the sprite renderer and set its color
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = puddleColor;
        }

        // Start the fade out coroutine
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

    protected override void OnCollide(Collider2D coll)
    {
        // If the collision was with the player
        if (coll.tag == "Fighter" && coll.name == "Player" && !isApplyingDamage)
        {
            // Apply damage and slow effect
            GameObject entity = coll.gameObject;

            // Start coroutine to apply damage over time
            StartCoroutine(ApplyDamageOverTime(entity));

            // Apply slow effect
            JoystickMove joystickMove = entity.GetComponent<JoystickMove>();
            if (joystickMove != null)
            {
                StartCoroutine(ApplySlowEffect(joystickMove));
            }
        }
    }

    private IEnumerator ApplyDamageOverTime(GameObject target)
    {
        isApplyingDamage = true;

        while (target != null && gameObject.activeSelf)
        {
            // Apply damage to the target
            Damage puddleDmg = new Damage
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = 0f, // No push force for puddle damage
                isCritical = false,
                minPossibleDamage = damage,
                maxPossibleDamage = damage,
                useCustomColor = true,
                customColor = new Color(puddleColor.r, puddleColor.g, puddleColor.b, 1f) // Make fully opaque for text
            };

            target.SendMessage("ReceiveDamage", puddleDmg);

            // Wait for the next tick
            yield return new WaitForSeconds(damageTickRate);

            // Check if the player is still colliding with the puddle
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                Collider2D[] results = new Collider2D[1];
                if (collider.Overlap(new ContactFilter2D().NoFilter(), results) == 0 || results[0] != target.GetComponent<Collider2D>())
                {
                    // Player no longer in puddle
                    break;
                }
            }
        }

        isApplyingDamage = false;
    }

    private IEnumerator ApplySlowEffect(JoystickMove joystickMove)
    {
        // Remember the original speed
        float originalSpeed = joystickMove.playerSpeed;

        // Apply the slow effect
        joystickMove.playerSpeed *= slowFactor;

        // Wait for a short time
        yield return new WaitForSeconds(damageTickRate * 2); // Give a bit more time for slow effect

        // Restore the original speed if the player is not in another puddle
        joystickMove.playerSpeed = originalSpeed;
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