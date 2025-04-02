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
    
    [Header ("Phantom setting")] 
    public bool isPhantom = false;

    // Hitbox
    public ContactFilter2D filter;
    public BoxCollider2D hitBox;
    public Collider2D[] hits = new Collider2D[10];

    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;
        hitBox = transform.GetChild(0).GetComponent<BoxCollider2D>();

        if (isPhantom)
        {
            hitpoint = 1;
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        }
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
                    UpdateMotor((playerTransform.position - transform.position).normalized);
                }
            }
            else
            {
                UpdateMotor(startingPosition - transform.position);
            }
        }
        else
        {
            UpdateMotor(startingPosition - transform.position);
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
        int xp = ExperienceManager.Instance.AddEnemy(1);
        GameSummaryManager.Instance.AddEnemy();
        // GameManager.instance.XpManager.Experience += xpValue;
        GameManager.instance.ShowText("+" + xp + " xp", 10, Color.magenta, transform.position, Vector3.up * 1, 1.0f);
        }
        else
        {
            GameManager.instance.ShowText("Phantom vanished", 10, Color.gray, transform.position, Vector3.up * 1, 1.0f);
        }
        
        Destroy(gameObject);
    }
}