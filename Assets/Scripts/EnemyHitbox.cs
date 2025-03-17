using UnityEngine;

public class EnemyHitbox : Collidable
{
    public float pushForce = 5.0f;
    public int minDamage = 1;
    public int maxDamage = 1;
    public float critChance = 0.1f; // 10% chance
    public float critMultiplier = 1.5f;

    private System.Random random = new System.Random();

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter" && coll.name == "Player")
        {
            int actualDamage = random.Next(minDamage, maxDamage + 1);
            bool isCritical = (random.NextDouble() < critChance);

            if (isCritical)
            {
                actualDamage = Mathf.RoundToInt(actualDamage * critMultiplier);
            }

            Damage dmg = new Damage
            {
                damageAmount = actualDamage,
                origin = transform.position,
                pushForce = pushForce,
                isCritical = isCritical,
                minPossibleDamage = minDamage,
                maxPossibleDamage = maxDamage
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }
}