using System;
using System.Collections.Generic;
using UnityEngine;

public class PerksManager : MonoBehaviour
{
    public static PerksManager Instance;

    private int _points;

    public int Points
    {
        get => _points;
        set
        {
            _points = value;
            StateManager.Instance.SaveState();
        }
    }

    public Dictionary<String, IPerk> Perks = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePerks();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePerks()
    {
        // Add default perks
        var healthPerk = new HealthPerk();
        var experiencePerk = new ExperiencePerk();
        var attackSpeedPerk = new AttackSpeedPerk();
        var movementSpeedPerk = new MovementSpeedPerk();
        var criticalChancePerk = new CriticalChancePerk();
        var attackDamagePerk = new AttackDamagePerk();

        Perks.Add(healthPerk.Name, healthPerk);
        Perks.Add(experiencePerk.Name, experiencePerk);
        Perks.Add(attackSpeedPerk.Name, attackSpeedPerk);
        Perks.Add(movementSpeedPerk.Name, movementSpeedPerk);
        Perks.Add(criticalChancePerk.Name, criticalChancePerk);
        Perks.Add(attackDamagePerk.Name, attackDamagePerk);
    }

    public void Upgrade(IPerk perk)
    {
        if (!Perks.ContainsKey(perk.Name))
        {
            Debug.Log("Perk not found in the list.");
            return;
        }

        if (Points < perk.Cost)
        {
            Debug.Log("Not enough points to upgrade this perk.");
            return;
        }

        Points -= perk.Cost;
        perk.Upgrade();
        StateManager.Instance.SaveState();
    }

    public void AddPoints(int points)
    {
        Points += points;
    }

    /// <summary>
    /// Apply all perks to the current player
    /// Called when a new game starts
    /// </summary>
    public void ApplyPerksToPlayer()
    {
        if (GameManager.Instance?.player == null) return;

        GameManager.Instance.player.playerStats.Reset();

        // Apply all active perks
        foreach (var perk in Perks.Values)
        {
            if (perk.Level > 0)
            {
                perk.Apply();
            }
        }

        Debug.Log("Applied all perks to player");
    }
}