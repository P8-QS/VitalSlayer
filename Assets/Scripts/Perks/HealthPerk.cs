using System;
using UnityEngine;

public class HealthPerk : IPerk
{
    public string Name => "Health";
    public int Cost { get; set; }
    public int Level { get; set; }

    public string Description()
    {
        return $"You will get <b>{Level}% more</b> health points.";
    }


    public void Apply()
    {
        throw new NotImplementedException();
    }
}