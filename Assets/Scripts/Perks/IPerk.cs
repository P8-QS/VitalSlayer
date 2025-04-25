using System;
using UnityEngine;

public interface IPerk
{
    public string Name { get; }
    public int Cost { get; set; }
    public int Level { get; set; }
    public string Description();
    public void Apply();
    
    public void Upgrade()
    {
        Level++;
        Cost++;
    }
}