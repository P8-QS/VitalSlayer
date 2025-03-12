using UnityEngine;

public class Player : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private JoystickMove joystickMove;
    private Vector3 initialSize;

    public string playerName;
    public int playerHealth;
    public int playerAttack;
    public int xp;
    


    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        joystickMove = GetComponent<JoystickMove>();
        initialSize = transform.localScale;

        // player attributes
        playerHealth = 100;
        playerAttack = 10;
        xp = 0;
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

    public void SetPlayerName(string name)
    {
        playerName = name;
    }
    
    public void TakeDmg(int dmg){
        playerHealth -= dmg;
        if(playerHealth <= 0){
            playerHealth = 0;
           // Debug.Log("Player is dead");
        }
    }

    public void GainXp(int xpAmount){
        xp += xpAmount;
    }

    public void Attack(Player target){
        target.TakeDmg(playerAttack);
    }
    
}


