using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : Mover
{
    public Animator animator;
    private JoystickMove joystickMove;

    private float hitAnimationTimer = 0f;
    private const float HIT_ANIMATION_DURATION = 0.15f; // Duration in seconds

    protected override void Start()
    {
        currentLevel = ExperienceManager.Instance.Level;
        maxHitpoint = 100 + (int)(25 + Mathf.Pow(Fighter.currentLevel, 1.2f));
        hitpoint = maxHitpoint;
        base.Start();
        boxCollider = GetComponent<BoxCollider2D>();
        joystickMove = GetComponent<JoystickMove>();
        initialSize = transform.localScale;

        // Get the animator component if not already assigned in Inspector
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        currentLevel = ExperienceManager.Instance.Level;
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
    }

    protected override void ReceiveDamage(Damage dmg)
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