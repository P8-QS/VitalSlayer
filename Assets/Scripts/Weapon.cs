using UnityEngine;

public class Weapon : Collidable
{
    public float basePushForce = 2.0f;
    public float critChance = 0.1f; // 10% chances
    public float critMultiplier = 2.0f;

    public int weaponLevel;
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
        int fighterLvl = Fighter.currentLevel;
        float weaponDmg = 10f + (float)Mathf.Floor(4 * Mathf.Pow(fighterLvl, 1.2f));

        int minDamage = Mathf.RoundToInt(weaponDmg * 0.5f);
        int maxDamage = Mathf.RoundToInt(weaponDmg * 1.5f);

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
