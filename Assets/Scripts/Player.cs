using UnityEngine;

public class Player : Mover
{
    private JoystickMove joystickMove;
    
    protected override void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        joystickMove = GetComponent<JoystickMove>();
        initialSize = transform.localScale;
    }

    private void FixedUpdate()
    {
        Vector3 input = new Vector3(joystickMove.movementJoystick.Direction.x, joystickMove.movementJoystick.Direction.y, 0);
        
        UpdateMotor(input);
    }
    
}


