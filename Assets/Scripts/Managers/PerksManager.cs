using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PerksManager : MonoBehaviour
{
    private static PerksManager _instance;
    public static PerksManager Instance => _instance ??= new PerksManager();

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

    private PerksManager()
    {
        // Add default perks
        var healthPerk = new HealthPerk();
        var experiencePerk = new ExperiencePerk();
        var attackSpeedPerk = new AttackSpeedPerk();

        Perks.Add(healthPerk.Name, healthPerk);
        Perks.Add(experiencePerk.Name, experiencePerk);
        Perks.Add(attackSpeedPerk.Name, attackSpeedPerk);
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
}