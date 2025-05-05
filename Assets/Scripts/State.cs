using System;
using System.Collections.Generic;
using UnityEngine.Serialization;


[Serializable]
public struct Perk
{
    public string name;
    public int cost;
    public int level;
}

[Serializable]
public struct State
{
    public int experience;
    public int points;
    public List<Perk> perks;
    public bool promptForEmail;
    public string email;
}