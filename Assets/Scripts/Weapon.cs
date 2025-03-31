using UnityEngine;

public class Weapon : Collidable
{
    // Scaling + base stats
    public int baseMinDamage = 1;
    public int baseMaxDamage = 5;
    public float damageScalingFactor = 1.15f;

    public float basePushForce = 2.0f;
    public float critChance = 0.1f; // 10% chances
    public float critMultiplier = 2.0f;

    public bool canAttack = false;

    public SpriteRenderer spriteRenderer;

    private System.Random random = new System.Random();

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollide(Collider2D other)
    {
        if (canAttack)
        {
            Damage damage = calcWeaponDmg();

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ReceiveDamage(damage);
            }
        }
    }

    private Damage calcWeaponDmg()
    {
        // Currnet we only apply weapons for players- maybe needs to be fixed later?
        int playerLevel = ExperienceManager.Instance.Level;

        int minDamage = GameHelpers.CalculateDamageStat(baseMinDamage, playerLevel, damageScalingFactor);
        int maxDamage = GameHelpers.CalculateDamageStat(baseMaxDamage, playerLevel, damageScalingFactor);

        int damageAmount = random.Next(minDamage, maxDamage + 1);
        bool isCritical = random.NextDouble() < critChance;

        if (isCritical)
        {
            damageAmount = Mathf.RoundToInt(damageAmount * critMultiplier);
        }

        float pushForce = basePushForce * (damageAmount / (float)maxDamage);

        Damage result = new Damage
        {
            damageAmount = damageAmount,
            origin = transform.position,
            pushForce = pushForce,
            isCritical = isCritical,
            minPossibleDamage = minDamage,
            maxPossibleDamage = maxDamage,
            useCustomColor = false,
        };

        return result;
    }
}
