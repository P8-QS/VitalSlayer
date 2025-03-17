using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Player _player; 
    public Text playerNameTxt;
    public Text level;
    public Text xp;
    public Text playerHealth;
    public Text playerAttack;
    public Text xpToNextLevel;

    private void Start()
    {
        // Find the _player script in the scene
        _player = FindFirstObjectByType<Player>();

        // Make sure we found a _player before continuing
        if (_player == null)
        {
            Debug.LogError("_playerUI: No _player script found in the scene!");
            return;
        }
        UpdateUI();
    }

    private void Update()
    {
        if (_player)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (_player == null) return; 

        playerNameTxt.text = _player.playerName;
        level.text = "Level: " + _player.GetLevel();
        level.text = "Level: " + _player.level;
        xp.text = "XP: " + _player.xp;
        playerHealth.text = "Health: " + _player.playerHealth;
        playerAttack.text = "Attack: " + _player.playerAttack;
        xpToNextLevel.text = "XP to next level: " + _player.xPToNextLevel;
       
    }
}