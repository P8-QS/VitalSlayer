using Unity.VisualScripting;
using UnityEngine;

public class EnemyHitbox : Collidable
{
    [DoNotSerialize] public Enemy enemy;

    protected System.Random random = new System.Random();

    protected override void Start()
    {
        base.Start();
        enemy = GetComponentInParent<Enemy>();
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            Damage dmg = enemy.enemyStats.CalculateDamageObject(enemy.level, transform.position);
            coll.SendMessage("ReceiveDamage", dmg);
        }
    }
}