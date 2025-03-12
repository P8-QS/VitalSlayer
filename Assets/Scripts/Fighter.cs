using UnityEngine;

public class Fighter : MonoBehaviour
{
    // Public variables
    public int hitpoint = 10;
    public int maxHitPopiunt = 10;
    public float pushRecoverySpeed = 0.2f;

    // Immunity
    private float immuneTime = 1.0f;
    private float lastImmune;

    // Push
    private Vector2 pushDirection;
    
    
    protected virtual void ReceiveDamage(Damage dmg)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            hitpoint -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
            Debug.Log($"TEST POSITION IS {transform.position}");
            GameManager.instance.ShowText(dmg.damageAmount.ToString(), 20, Color.white, transform.position, Vector3.zero, 0.5f);

            if (hitpoint <= 0)
            {
                hitpoint = 0;
                //Death();
            }
        }
    }
}
