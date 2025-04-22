using Metrics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player : Mover
{
    [Header("Stats")]
    [SerializeField] private BaseFighterStats playerStats;

    [Header("References")]
    public Animator animator;

    [Header("Combat Settings")]
    public float attackCooldown = 2.0f;
    [DoNotSerialize]
    public float lastAttackTime = 0f;

    private Weapon weapon;
    private Animator weaponAnimator;
    public Joystick movementJoystick;
    private float hitAnimationTimer = 0f;
    private const float HIT_ANIMATION_DURATION = 0.15f;

    protected override void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError("Player stats not assigned to " + gameObject.name);
            return;
        }
        SetStats(playerStats);

        level = ExperienceManager.Instance.Level;
        base.Start();
        boxCollider = GetComponent<BoxCollider2D>();
        initialSize = transform.localScale;
        GameObject weaponObj = transform.Find("weapon_00").gameObject;
        weapon = weaponObj.GetComponent<Weapon>();
        weaponAnimator = weaponObj.GetComponent<Animator>();
        movementJoystick = GameObject.Find("Canvas").transform.Find("Safe Area").Find("Variable Joystick")
            .GetComponent<Joystick>();

        var metrics = MetricsManager.Instance?.metrics.Values;
        if (metrics != null)
        {
            foreach (var metric in metrics)
            {
                metric.Effect.Apply();
            }
        }

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Show level above player
        FloatingTextManager.Instance.Show("Level " + level, 6, Color.white, transform.position + Vector3.up * 0.2f,
            Vector3.zero, 0.0001f);

        level = ExperienceManager.Instance.Level;
        if (hitAnimationTimer > 0)
        {
            hitAnimationTimer -= Time.deltaTime;
            if (hitAnimationTimer <= 0)
            {
                animator.SetBool("hit", false);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 input = new Vector3(movementJoystick.Direction.x * currentSpeed, movementJoystick.Direction.y * currentSpeed, 0);
        Animate(input);
        UpdateMotor(input);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    public void Attack()
    {
        SoundFxManager.Instance.PlaySound(stats.hitSound, transform, 0.8f);
        lastAttackTime = Time.time;
        weaponAnimator.SetTrigger("Attack");
        weapon.canAttack = true;
        Invoke(nameof(DisableWeaponCollider), 0.3f);
    }

    public void DisableWeaponCollider()
    {
        weapon.canAttack = false;
    }

    public override void ReceiveDamage(Damage dmg)
    {
        int previousHitpoints = hitpoint;

        base.ReceiveDamage(dmg);

        if (hitpoint < previousHitpoints && animator != null)
        {
            animator.SetBool("hit", true);
            hitAnimationTimer = HIT_ANIMATION_DURATION;
        }
    }

    protected override void Death()
    {
        SoundFxManager.Instance.PlaySound(stats.deathSound, 1f);
        Destroy(gameObject);
        GameSummaryManager.Instance.Show();
    }

    private void Animate(Vector3 input)
    {
        if (animator != null)
        {
            if (hitAnimationTimer <= 0)
            {
                float magnitude = input.magnitude;
                animator.SetBool("moving", magnitude > 0.1f);
            }
        }
        else
        {
            Debug.LogWarning("Animator component not found on Player!");
        }
    }
}