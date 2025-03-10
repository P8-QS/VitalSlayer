using UnityEngine;

public class Player : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private JoystickMove joystickMove;
    private Vector3 initialSize;
    


    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        joystickMove = GetComponent<JoystickMove>();
        initialSize = transform.localScale;
    }

    private void FixedUpdate()
    {
        float moveDirection = joystickMove.movementJoystick.Direction.x;
        
        if (moveDirection < 0)
        {
            transform.localScale = new Vector3(-initialSize.x, initialSize.y, initialSize.z);
        }
        else if (moveDirection > 0)
        {
            transform.localScale = new Vector3(initialSize.x, initialSize.y, initialSize.z);
        }
    }
    
}


