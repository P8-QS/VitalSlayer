using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public Vector3Int trapCellPosition;
    public TrapManager trapManager;

    private float lastTriggerTime = -Mathf.Infinity;
    public float triggerCooldown = 1.5f; // seconds between activations

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time - lastTriggerTime >= triggerCooldown)
        {
            lastTriggerTime = Time.time;
            trapManager.TriggerTrapAt(trapCellPosition, other.gameObject);
        }
    }
}
