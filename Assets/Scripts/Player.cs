using UnityEngine;
using 

public class Player : MonoBehaviour,
{
    private BoxCollider2D boxCollider;
    private JoystickMove joystickMove;
    private Vector3 initialSize;

    public string playerName;
    public double playerHealth;
    public int playerAttack;
    public int xp;
    public int level;
    public int levelRange;
    // Dunno how to handle "bot enemies" yet
    public Enemy enemy;
    public Enemy boss;
    // This is just a placeholder for now
    // i dont know how to we are going to handle difficulty of yet
//public double questDifficulty = 2 + (Player.level * 0.8)
    


    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        joystickMove = GetComponent<JoystickMove>();
        initialSize = transform.localScale;

        // player attributes
        playerHealth = setPlayerHealth();
        playerAttack = setPlayerAttack(Player player);
        level = 1;
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

    private void Update()
    {
        // update player level, xp and name such that when player leaves the game and comes back, the player will still have the same level, xp and name
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetFloat("PlayerHealth", (float)playerHealth);
        PlayerPrefs.SetInt("PlayerAttack", playerAttack);
        PlayerPrefs.SetInt("PlayerLevel", level);
        PlayerPrefs.SetInt("PlayerXP", xp);
    }
    
    /* public void TakeDmg(int dmg){
        playerHealth -= dmg;
        if(playerHealth <= 0){
            playerHealth = 0;
           // Debug.Log("Player is dead");
        }
    } */

    public void SetPlayerName(string name)
    {
        playerName = name;
    }
    

    public double SetPlayerHealth(){
      return Math.Floor(100 + (25 *  Math.Pow(level, 1.2)));
    }

    public void GainXpByFighting(Enemy enemy)
    {
        double enemyXP = 10 * Math.Pow(enemy.level, 2);
        double bossXP = 30 * Math.Pow(enemy.level, 2);

        if (enemy is Boss)
        {
            xp += (int)bossXP;
        }
        else
        {
            xp += (int)enemyXP;
        }
    }

    public void QuestXp(){
        // quest difficulty calcluation = 2 + (player.level * 0.8)
        double questDifficulty = 2 + (level * 0.8);
        double qXP = 50 + (20 + questDifficulty)
        xp += qXP;
    }
    public double SetPlayerAttack(){
        // dunno if attack should be ints our doubles like 20.1 or just 20
       return Math.Floor(10 + (4 * Math.Pow(level, 1.4)));
    }
    
    public void LevelUp(){
        // level range until next level
        levelRange = Math.Floor(100 * Math.Pow(level, 1.2));
        if(xp >= levelRange){
            level++;
            xp = 0;
            playerHealth += SetPlayerHealth();
            playerAttack += SetPlayerAttack();
        }
       
    }
    
}


