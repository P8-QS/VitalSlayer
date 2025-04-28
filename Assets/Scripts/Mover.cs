using UnityEngine;

public abstract class Mover : Fighter
{
    protected Vector3 initialSize;
    protected BoxCollider2D boxCollider;
    protected Vector3 moveDelta;
    protected RaycastHit2D hit;
    protected float ySpeed = 0.75f;
    protected float xSpeed = 1.0f;

    protected override void Start()
    {
        base.Start(); // Call Fighter's Start method
        boxCollider = GetComponent<BoxCollider2D>();
        initialSize = transform.localScale;
    }

    protected virtual void UpdateMotor(Vector3 input)
    {
        // Reset moveDelta
        moveDelta = input;

        // Swap sprite direction, whether it's going right or left
        if (moveDelta.x > 0)
        {
            transform.localScale = new Vector3(initialSize.x, initialSize.y, initialSize.z);
        }
        else if (moveDelta.x < 0)
        {
            transform.localScale = new Vector3(-initialSize.x, initialSize.y, initialSize.z);
        }

        // Add push vector, reduce it over time
        moveDelta += pushDirection;

        float pushRecovery = stats != null ? stats.pushRecoverySpeed : 0.2f;

        // Reduce push force 
        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecovery);

        // Make sure we can move in this direction by casting a box there first, if the box returns null, we're free to move
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Default", "Blocking"));
        if (hit.collider == null)
        {
            // Make this move
            transform.Translate(0, moveDelta.y * Time.deltaTime, 0);
        }

        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Default", "Blocking"));
        if (hit.collider == null)
        {
            // Make this move
            transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);
        }
    }
}