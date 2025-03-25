using UnityEngine;
using TMPro;

public class SummaryItem : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemValue;

    public void SetValue(string name, string value)
    {
        itemName.text = name;
        itemValue.text = value;
    }
}