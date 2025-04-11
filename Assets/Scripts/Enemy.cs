using UnityEngine;
using Effects;

public class Enemy : Mover
{
    // Logic
    public float triggerLength = 1;
    public float chaseLength = 5;
    protected bool chasing;
    protected bool collidingWithPlayer;
    protected Transform playerTransform;
    protected Vector3 startingPosition;
    [Header("Phantom setting")] public bool isPhantom;

    // Hitbox
    public ContactFilter2D filter;
    public BoxCollider2D hitBox;
    public Collider2D[] hits = new Collider2D[10];

    public AudioClip deathSound;


    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.Instance.player.transform;
        startingPosition = transform.position;
        hitBox = transform.GetChild(0).GetComponent<BoxCollider2D>();

    }

    protected void FixedUpdate()
    {
        if (!playerTransform) return;

        // Is the player in range?
        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseLength)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerLength)
            {
                chasing = true;
            }

            if (chasing)
            {
                if (!collidingWithPlayer)
                {
                    UpdateMotor((playerTransform.position - transform.position).normalized * currentSpeed);
                }
            }
            else
            {
                UpdateMotor((startingPosition - transform.position) * currentSpeed);
            }
        }
        else
        {
            UpdateMotor((startingPosition - transform.position) * currentSpeed);
            chasing = false;
        }

        // Check for overlaps
        collidingWithPlayer = false;
        boxCollider.Overlap(filter, hits);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
            {
                continue;
            }

            if (hits[i].CompareTag("Player"))
            {
                collidingWithPlayer = true;
            }

            // Reset array
            hits[i] = null;
        }
    }


    protected override void Death()
    {
        if (!isPhantom)
        {
            SoundFxManager.Instance.PlaySound(deathSound, 0.5f);
            int xp = ExperienceManager.Instance.AddEnemy(1);
            GameSummaryManager.Instance.AddEnemy();
            // GameManager.instance.XpManager.Experience += xpValue;
            FloatingTextManager.Instance.Show("+" + xp + " xp", 10, Color.magenta, transform.position, Vector3.up * 1, 1.0f);
        }
        else
        {
            SoundFxManager.Instance.PlaySound(deathSound, 0.5f);
            FloatingTextManager.Instance.Show("Phantom vanished", 10, Color.gray, transform.position, Vector3.up * 1, 1.0f);
        }

        Destroy(gameObject);

    }
}