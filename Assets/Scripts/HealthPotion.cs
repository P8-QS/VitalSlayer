using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public float healAmount = 0.25f;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                if (player.hitpoint < player.maxHitpoint)
                {   
                    float maxHpFloat = player.maxHitpoint;
                    
                    int calculatedHealAmount = Mathf.RoundToInt(healAmount * maxHpFloat);

                    int actualHealAmount = Mathf.Min(calculatedHealAmount, player.maxHitpoint - player.hitpoint);

                    player.hitpoint += actualHealAmount;
                    
                    // Show healing effect
                    FloatingTextManager.Instance.Show("+" + actualHealAmount.ToString(), 25, Color.green, 
                        transform.position, Vector3.up * 30, 1.0f);
                        
                    Destroy(gameObject);
                }
            }
        }
    }
}