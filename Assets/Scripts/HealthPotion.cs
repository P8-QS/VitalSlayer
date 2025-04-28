using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public double healAmount = 1.25f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player collided with health potion");

            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                // Check if player isn't already at max health
                if (player.hitpoint < player.maxHitpoint)
                {
                    // Create a healing damage (negative damage)
                    Damage healingEffect = new Damage
                    {
                        damageAmount = (int)(-healAmount*player.maxHitpoint),  // Negative damage = healing
                        origin = transform.position,
                        pushForce = 0f,
                        useCustomColor = true,
                        customColor = Color.green
                    };
                    
                    // Apply healing through the damage system
                    player.ReceiveDamage(healingEffect);
                    
                    // Show healing effect
                    FloatingTextManager.Instance.Show("+" + healAmount.ToString(), 25, Color.green, 
                        transform.position, Vector3.up * 30, 1.0f);
                    
                    // Play sound effect if available
                    // if (SoundFxManager.Instance != null)
                    //     SoundFxManager.Instance.PlaySound("heal", transform, 0.8f);
                        
                    Destroy(gameObject);
                }
                else
                {
                    // Optional: Show message that player is already at max health
                    FloatingTextManager.Instance.Show("Max Health!", 15, Color.yellow, 
                        transform.position, Vector3.up * 30, 1.0f);
                }
            }
        }
    }
}