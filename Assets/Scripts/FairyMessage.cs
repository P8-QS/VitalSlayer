using UnityEngine;

[System.Serializable]
public class FairyMessage
{
    [TextArea(3, 10)]
    [Tooltip("You can use rich text tags for formatting. Examples:\n" +
             "<color=red>Red text</color>\n" +
             "<color=#00FF00>Green text</color>\n" +
             "<b>Bold text</b>\n" +
             "<i>Italic text</i>\n" +
             "<size=20>Larger text</size>")]
    public string message;
    public float cooldownDuration = 30f;
    [Tooltip("Assign a tag to categorize this message (e.g., 'Sleep', 'Exercise', 'Heart')")]
    public string tag;
}