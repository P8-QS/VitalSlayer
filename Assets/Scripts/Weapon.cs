using UnityEngine;

public class Weapon : Collidable
{
    [HideInInspector] public bool canAttack = false;
    public SpriteRenderer spriteRenderer;
    private Player player;
    private PlayerStats playerStats;
    private System.Random random = new System.Random();

    protected override void Start()
    {
        base.Start();

        // Find parent Player component
        player = GetComponentInParent<Player>();
        if (player == null)
        {
            Debug.LogError("Player component not found in parent of " + gameObject.name);
            return;
        }
        playerStats = player.playerStats;
    }

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
                SoundFxManager.Instance.PlaySound(playerStats.enemyHitSound, transform, 0.25f);
            }
        }
    }

    private Damage calcWeaponDmg()
    {
        int playerLevel = ExperienceManager.Instance.Level;

        int minDamage = playerStats.CalculateMinDamage(playerLevel);
        int maxDamage = playerStats.CalculateMaxDamage(playerLevel);

        int damageAmount = random.Next(minDamage, maxDamage + 1);
        bool isCritical = random.NextDouble() < playerStats.critChance;

        if (isCritical)
        {
            damageAmount = Mathf.RoundToInt(damageAmount * playerStats.critMultiplier);
        }

        float pushForce = playerStats.pushForce * (damageAmount / (float)maxDamage);

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
