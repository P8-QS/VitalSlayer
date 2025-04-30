using System.Linq;
using Effects;
using Managers;
using NUnit.Framework;
using UnityEngine;

public class Player : Mover
{
    [Header("Stats")] public PlayerStats playerStats;

    [Header("References")] public Animator animator;

    private float lastAttackTime = 0f;
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
        weapon = GetComponentInChildren<Weapon>();

        level = ExperienceManager.Instance.Level;
        base.Start();

        boxCollider = GetComponent<BoxCollider2D>();
        initialSize = transform.localScale;

        GameObject weaponObj = transform.Find("weapon_00").gameObject;

        weaponAnimator = weaponObj.GetComponent<Animator>();

        movementJoystick = GameObject.Find("Canvas").transform.Find("Safe Area").Find("Variable Joystick")
            .GetComponent<Joystick>();

        var metrics = MetricsManager.Instance?.metrics.Values;
        if (metrics != null)
        {
            foreach (var effect in metrics.SelectMany(metric => metric.Effects))
            {
                effect.Apply();
            }
        }

        var perks = PerksManager.Instance?.Perks.Values;
        if (perks != null)
        {
            foreach (var perk in perks)
            {
                perk.Apply();
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
        Vector3 input = new Vector3(movementJoystick.Direction.x * currentSpeed,
            movementJoystick.Direction.y * currentSpeed, 0);
        Animate(input);
        UpdateMotor(input);

        if (Time.time >= lastAttackTime + playerStats.attackCooldown)
        {
            Attack();
        }
    }

    private float GetModifiedAttackCooldown()
    {
        float baseCooldown = playerStats.attackCooldown;

        float modifier = StatsModifier.Instance.GetModifier(StatsModifier.StatType.AttackSpeed);

        return baseCooldown / modifier;
    }


    public void Attack()
    {
        SoundFxManager.Instance.PlaySound(playerStats.attackSound, transform, 0.8f);
        lastAttackTime = Time.time;

        float anim_length = GetWeaponAnimationClipLength("weapon_swing");

        if (GetModifiedAttackCooldown() < anim_length)
        {
            float anim_speed = anim_length / GetModifiedAttackCooldown();
            weaponAnimator.speed = anim_speed;
        }

        weaponAnimator.SetTrigger("Attack");

        weapon.canAttack = true;

        weapon.CreateSlashEffect();

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
        SoundFxManager.Instance.PlaySound(playerStats.deathSound, 1f);
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

    private float GetWeaponAnimationClipLength(string clipName)
    {
        foreach (var clip in weaponAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        Debug.LogWarning("Animation clip not found!");
        return 1f;
    }
}