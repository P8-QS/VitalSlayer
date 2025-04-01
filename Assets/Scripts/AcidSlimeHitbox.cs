using System.Collections;
using UnityEngine;

public class AcidSlimeHitbox : EnemyHitbox
{
    [Header("Acid Properties")]
    public int acidMinDamage = 1;
    public int acidMaxDamage = 1;
    public float acidDuration = 3.0f;
    public float acidTickRate = 1.0f;
    public Color acidColor = Color.green;

    protected override void OnCollide(Collider2D coll)
    {
        // First, trigger the normal damage from EnemyHitbox
        base.OnCollide(coll);

        // Then, if the collision was with the player, apply the acid effect
        if (coll.CompareTag("Player"))
        {
            // Apply the acid damage over time effect
            StartCoroutine(ApplyAcidDamageCoroutine(coll.gameObject));
        }
    }

    // Coroutine to apply damage over time
    private IEnumerator ApplyAcidDamageCoroutine(GameObject target)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < acidDuration)
        {
            // Wait for the next tick
            yield return new WaitForSeconds(acidTickRate);

            // Make sure the target still exists
            if (target == null)
                yield break;

            int damage = random.Next(acidMinDamage, acidMaxDamage + 1);

            // Apply damage to the target
            Damage acidDmg = new Damage
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = 0f, // No push force for acid damage
                isCritical = false,
                minPossibleDamage = acidMinDamage,
                maxPossibleDamage = acidMaxDamage,
                useCustomColor = true,
                customColor = acidColor
            };

            target.SendMessage("ReceiveDamage", acidDmg);

            elapsedTime += acidTickRate;
        }
    }
}