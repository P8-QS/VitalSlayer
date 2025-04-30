using UnityEngine;


public class Weapon : Collidable
{
    [HideInInspector] public bool canAttack = false;
    public SpriteRenderer spriteRenderer;

    [Header("Slash Effect")]
    public GameObject slashEffectPrefab;
    public Vector2 slashOffset = new Vector2(0.5f, 0f);

    private Player player;
    [HideInInspector] public RuntimePlayerStats playerStats;
    private System.Random random = new System.Random();

    protected override void Start()
    {
        base.Start();

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
        if (canAttack && slashEffectPrefab == null)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Damage damage = CalcWeaponDmg();
                enemy.ReceiveDamage(damage);
                SoundFxManager.Instance.PlaySound(playerStats.HitSound, transform, 0.25f);
            }
        }
    }

    public void CreateSlashEffect()
    {
        if (slashEffectPrefab == null)
        {
            Debug.LogWarning("No slash effect prefab assigned to weapon!");
            return;
        }

        bool isFacingRight = player.transform.localScale.x > 0;
        Vector3 facingDirection = isFacingRight ? Vector3.right : Vector3.left;

        Vector3 slashPosition = transform.position + new Vector3(
            slashOffset.x * facingDirection.x,
            isFacingRight ? slashOffset.y : -slashOffset.y,
            0
        );

        Quaternion slashRotation = Quaternion.Euler(0, 0, isFacingRight ? 90 : -90);
        GameObject slashObj = Instantiate(slashEffectPrefab, slashPosition, slashRotation, player.transform);

        SlashEffect slashEffect = slashObj.GetComponent<SlashEffect>();
        slashEffect.Initialize(this, player, slashPosition, CalcWeaponDmg());
    }

    public Damage CalcWeaponDmg()
    {
        int playerLevel = ExperienceManager.Instance.Level;

        int minDamage = playerStats.CalculateMinDamage(playerLevel);
        int maxDamage = playerStats.CalculateMaxDamage(playerLevel);

        int damageAmount = random.Next(minDamage, maxDamage + 1);
        bool isCritical = random.NextDouble() < playerStats.CritChance;

        if (isCritical)
        {
            damageAmount = Mathf.RoundToInt(damageAmount * playerStats.CritMultiplier);
        }

        float pushForce = playerStats.PushForce * (damageAmount / (float)maxDamage);

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