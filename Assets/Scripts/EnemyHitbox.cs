using UnityEngine;

public class EnemyHitbox : Collidable
{
    public Enemy enemy;

    public int baseMinDamage = 1;
    public int baseMaxDamage = 3;
    public float damageScalingFactor = 1.1f;

    // Current stats
    public float pushForce = 5.0f;
    public float critChance = 0.1f; // 10% chance
    public float critMultiplier = 1.5f;
    private int minDamage;
    private int maxDamage;

    protected System.Random random = new System.Random();

    protected override void Start()
    {
        base.Start();
        enemy = GetComponentInParent<Enemy>();
        UpdateStats();
    }

    protected virtual void UpdateStats()
    {
        // Set the calculated values to the collider
        this.minDamage = GameHelpers.CalculateDamageStat(baseMinDamage, enemy.level, damageScalingFactor);
        this.maxDamage = GameHelpers.CalculateDamageStat(baseMaxDamage, enemy.level, damageScalingFactor);
    }



    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter" && coll.name == "Player")
        {
            int actualDamage = random.Next(minDamage, maxDamage + 1);
            bool isCritical = random.NextDouble() < critChance;

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
                maxPossibleDamage = maxDamage,
                useCustomColor = false,
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }
}