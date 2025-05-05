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
        base.OnCollide(coll);

        if (coll.CompareTag("Player") && !enemy.isPhantom)
        {
            StartCoroutine(ApplyAcidDamageCoroutine(coll.gameObject));
        }
    }

    private IEnumerator ApplyAcidDamageCoroutine(GameObject target)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < acidDuration)
        {
            yield return new WaitForSeconds(acidTickRate);

            if (target == null)
                yield break;

            int damage = random.Next(acidMinDamage, acidMaxDamage + 1);

            Damage acidDmg = new Damage
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = 0f, // No push force for acid damage
                isCritical = false,
                minPossibleDamage = acidMinDamage,
                maxPossibleDamage = acidMaxDamage,
                useCustomColor = false,
            };

            target.SendMessage("ReceiveDamage", acidDmg);

            elapsedTime += acidTickRate;
        }
    }
}