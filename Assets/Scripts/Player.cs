using UnityEngine;

public class Player : Mover
{
    private JoystickMove joystickMove;
    
    protected override void Start()
    {
        //base.Start();
        boxCollider = GetComponent<BoxCollider2D>();
        joystickMove = GetComponent<JoystickMove>();
        initialSize = transform.localScale;
        getHP();
        getMaxHP();
    }

    private void FixedUpdate()
    {
        Vector3 input = new Vector3(joystickMove.movementJoystick.Direction.x, joystickMove.movementJoystick.Direction.y, 0);
        
        UpdateMotor(input);
    }

    protected override void Death()
    {
       Debug.Log("player died");
       Destroy(gameObject);
    }
    
}


