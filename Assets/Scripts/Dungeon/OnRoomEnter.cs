using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenTriggered) return;

        if (other.CompareTag("Player"))
        {
            RoomFogController room = GetComponentInParent<RoomFogController>();
            if (room != null)
            {
                room.RevealFog();
                hasBeenTriggered = true;
            }
        }
    }
}
