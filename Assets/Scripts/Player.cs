using UnityEngine;

public class Player : Mover
{
    public Animator animator;
    private JoystickMove joystickMove;
    public float attackCooldown = 2.0f;
    public float lastAttackTime = 0f;
    private Weapon weapon;
    private Animator weaponAnimator;

    private float hitAnimationTimer = 0f;
    private const float HIT_ANIMATION_DURATION = 0.15f; // Duration in seconds

    protected override void Start()
    {

        level = ExperienceManager.Instance.Level;
        maxHitpoint = 100 + (int)(25 + Mathf.Pow(level, 1.2f));
        hitpoint = maxHitpoint;
        base.Start();
        boxCollider = GetComponent<BoxCollider2D>();
        joystickMove = GetComponent<JoystickMove>();
        initialSize = transform.localScale;
        GameObject weaponObj = transform.Find("weapon_00").gameObject;
        weapon = weaponObj.GetComponent<Weapon>();
        weaponAnimator = weaponObj.GetComponent<Animator>();

        // Get the animator component if not already assigned in Inspector
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Show level above player
        // TODO: Er det her scuffed?
        GameManager.instance.ShowText("Level " + level, 20, Color.white, transform.position + Vector3.up * 0.6f, Vector3.zero, 0.0001f);

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
        Vector3 input = new Vector3(joystickMove.movementJoystick.Direction.x, joystickMove.movementJoystick.Direction.y, 0);
        Animate(input);
        UpdateMotor(input);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    public void Attack()
    {
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
        Debug.Log("player died");

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