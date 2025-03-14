using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Player _player; // Reference to the _player script
    public Text playerNameTxt;
    public Text level;
    public Text xp;
    public Text playerHealth;
    public Text playerAttack;
    public Text xpToNextLevel;

    private void Start()
    {
        // Find the _player script in the scene
        //_player = FindObjectOfType<_player>();
        _player = FindFirstObjectByType<Player>();

        // Make sure we found a _player before continuing
        if (_player == null)
        {
            Debug.LogError("_playerUI: No _player script found in the scene!");
            return;
        }

        // Initialize UI values
        UpdateUI();
    }

    private void Update()
    {
        if (_player)
        {
            UpdateUI();
            //Update_playerNameUI();
        }
    }

    private void UpdateUI()
    {
        if (_player == null) return; // Prevent null errors

        playerNameTxt.text = _player.playerName;
        level.text = "Level: " + _player.level;
        xp.text = "XP: " + _player.xp;
        playerHealth.text = "Health: " + _player.playerHealth;
        playerAttack.text = "Attack: " + _player.playerAttack;
        xpToNextLevel.text = "XP to next level: " + _player.xPToNextLevel;
       
    }

    // private void Update_playerNameUI()
    // {
    //     if (_player == null) return;
    //     
    //     _playerNameTxt.text = "_player: SVIN " + _player._playerName;
    //     
    //     Debug.LogError("_playerUI: _playerNameText UI element not assigned!");
    // }
    
    
}