using UnityEngine;
using System;
public class Player : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private JoystickMove joystickMove;
    private Vector3 initialSize;

    public string playerName;
    public float playerHealth;
    public float playerAttack;
    public float xp = 0.0f;
    public int level = 1;

    public float xPToNextLevel;
    // Dunno how to handle "bot enemies" yet
    public Enemy enemy;
    public Enemy boss;
    private float updateTimer = 0.0f;
    
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        joystickMove = GetComponent<JoystickMove>();
        initialSize = transform.localScale;

        // player attributes
        playerName = "Svin";
        level = 1;
        playerHealth = SetPlayerHealth();//(float) Math.Floor(100 + (25 * Math.Pow(level, 1.2)));//
        playerAttack = SetPlayerAttack(); //(float) Math.Floor(10 + (4 * Math.Pow(level, 1.4))); //setPlayerAttack()
        xp = 0;
        xPToNextLevel = (float) Math.Floor(100 * Math.Pow(level, 1.5));
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
    
    // Update is called once per frame, but we only want to update the player's stats every second
    private void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer > 1.0f)
        {
            updateTimer = 0.0f;
            Debug.Log("Update");
            
            // update player level, xp and name such that when player leaves the game and comes back, the player will still have the same level, xp and name
             PlayerPrefs.SetString("PlayerName", playerName);
             // debug log player name
             // Debug.Log(playerName);
             /*
             PlayerPrefs.SetFloat("PlayerHealth", (float)playerHealth);
             PlayerPrefs.SetFloat("PlayerAttack", playerAttack);
             PlayerPrefs.SetInt("PlayerLevel", level);
             PlayerPrefs.SetFloat("PlayerXP", xp); */
        }
    
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
    

    private float SetPlayerHealth(){
      return (float)Math.Floor(100 + (25 *  Math.Pow(level, 1.2)));
    }

    public void GainXpByFighting(Enemy enemy)
    {
        double enemyXP = 10 * Math.Pow(enemy.level, 2);
        double bossXP = 30 * Math.Pow(enemy.level, 2);

        if (enemy == boss)
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
        float questDifficulty = (float) (2 + (level * 0.8));
        float questExperience = 50 + (20 + questDifficulty);
        xp += questExperience;
    }
    public float SetPlayerAttack(){
        // dunno if attack should be ints our doubles like 20.1 or just 20
       return (float) Math.Floor(10 + (4 * Math.Pow(level, 1.4)));
    }
    
    public float GetNextLevel()
    {
        xPToNextLevel = (float)Math.Floor(100 * Math.Pow(level, 1.5));
        Debug.Log("XP to Next Level Calculated: " + xPToNextLevel);
        return xPToNextLevel; 
    }

    
    public void LevelUp()
    {
        float nextLevel = GetNextLevel();
        if(xp >= xPToNextLevel){
            level++;
            xp = 0;
            playerHealth += (float)SetPlayerHealth();
            playerAttack += (float)SetPlayerAttack();
        }
        
    }
    
}


