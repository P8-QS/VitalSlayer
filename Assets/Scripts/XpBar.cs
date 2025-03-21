using System;
using TMPro;
using UnityEngine;

public class XpBar : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void Start()
    {
        var xpManager = GameManager.instance.XpManager;
        SetXp(xpManager.Experience,xpManager.ExperienceMax);
    }
    
    public void Update()
    {
        var xpManager = GameManager.instance.XpManager;
        SetXp(xpManager.Experience,xpManager.ExperienceMax);
    }

    private void SetXp(int xp, int maxXp)
    {
        text.text = $"{xp}/{maxXp}";
    }
}
