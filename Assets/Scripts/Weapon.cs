using UnityEngine;

public class Weapon : Collidable
{

    public int damagePoint = 1;
    public float pushForce = 2.0f;

    public int weaponLevel = 0;
    public SpriteRenderer spriteRenderer;

    private float ccooldown = 0.5f;
    private float lastSwing;

    protected override void Update()
    {
        base.Update();
        if (Time.time - lastSwing > ccooldown)
        {
            lastSwing = Time.time;
            Swing();
        }
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter")
        {
            if (coll.name != "Player")
            {
                Damage dmg = new Damage
                {
                    damageAmount = damagePoint,
                    origin = transform.position,
                    pushForce = pushForce
                };
                coll.SendMessage("ReceiveDamage", dmg);
            }
        }
    }

    private void Swing()
    {
    }

}
