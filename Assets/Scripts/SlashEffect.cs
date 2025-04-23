using System.Collections;
using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    [Header("Slash Settings")]
    public float lifeTime = 0.3f;
    public bool canDamageEnemies = true;
    public LayerMask enemyLayers;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private Weapon sourceWeapon;
    private BoxCollider2D hitBox;
    private Damage damage;
    private bool hasDealtDamage = false;

    private void Awake()
    {
        hitBox = GetComponent<BoxCollider2D>();
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        if (!animator) animator = GetComponent<Animator>();
    }

    public void Initialize(Weapon weapon, Vector3 position, Damage dmg)
    {
        sourceWeapon = weapon;
        transform.position = position;
        damage = dmg;

        StartCoroutine(LifeSpan());
    }

    private IEnumerator LifeSpan()
    {
        yield return new WaitForSeconds(0.05f);

        if (canDamageEnemies && !hasDealtDamage)
        {
            DealDamageToEnemiesInRange();
            hasDealtDamage = true;
        }

        yield return new WaitForSeconds(lifeTime - 0.05f);

        Destroy(gameObject);
    }

    private void DealDamageToEnemiesInRange()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            transform.position,
            hitBox.size,
            transform.rotation.eulerAngles.z,
            enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy == null) continue;

            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.ReceiveDamage(damage);

                SoundFxManager.Instance.PlaySound(sourceWeapon.playerStats.hitSound, transform, 0.25f);
            }
        }
    }

}