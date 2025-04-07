using UnityEngine;

public class Enemy : Mover
{
    // Logic
    public float triggerLength = 1;
    public float chaseLength = 5;
    private bool chasing;
    private bool collidingWithPlayer;
    private Transform playerTransform;
    private Vector3 startingPosition;

    // Hitbox
    public ContactFilter2D filter;
    public BoxCollider2D hitBox;
    public Collider2D[] hits = new Collider2D[10];
    
    public AudioClip deathSound;


    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;
        hitBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    protected void FixedUpdate()
    {
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
        SoundFxManager.Instance.PlaySoundAtGlobal(deathSound, 1f);
        Destroy(gameObject);
        int xp = ExperienceManager.Instance.AddEnemy(1);
        GameSummaryManager.Instance.AddEnemy();
        // GameManager.instance.XpManager.Experience += xpValue;
        GameManager.instance.ShowText("+" + xp + " xp", 10, Color.magenta, transform.position, Vector3.up * 1, 1.0f);
    }
}