using UnityEngine;

public class Player : Mover
{
    private JoystickMove joystickMove;
    public float attackCooldown = 2.0f;
    public float lastAttackTime = 0f;
    private Weapon weapon;
    private Animator weaponAnimator;
    protected override void Start()
    {
        //base.Start();
        boxCollider = GetComponent<BoxCollider2D>();
        joystickMove = GetComponent<JoystickMove>();
        initialSize = transform.localScale;
        GameObject weaponObj = transform.Find("weapon_00").gameObject;
        weapon = weaponObj.GetComponent<Weapon>();
        weaponAnimator = weaponObj.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Vector3 input = new Vector3(joystickMove.movementJoystick.Direction.x, joystickMove.movementJoystick.Direction.y, 0);
        UpdateMotor(input);
        
        if (Time.time >= lastAttackTime + attackCooldown) {
            Attack();
        }
    }

    public void Attack() {
        lastAttackTime = Time.time;
        weaponAnimator.SetTrigger("Attack");
        weapon.canAttack = true;
        Invoke(nameof(DisableWeaponCollider),  0.1f);
    }

    public void DisableWeaponCollider() {
        weapon.canAttack = false;
    }

    protected override void Death()
    {
        Debug.Log("player died");
        Destroy(gameObject);
    }

}


