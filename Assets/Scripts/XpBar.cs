using TMPro;
using UnityEngine;

public class XpBar : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void Start()
    {
        SetXpValues();
    }

    public void Update()
    {
        SetXpValues();
    }

    private void SetXpValues()
    {
        SetXpBarText(ExperienceManager.Instance.Experience, ExperienceManager.Instance.ExperienceMax);
    }
    
    private void SetXpBarText(int xp, int maxXp)
    {
        text.text = $"{xp}/{maxXp}";
    }
}
