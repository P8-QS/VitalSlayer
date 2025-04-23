using TMPro;
using UnityEngine;
public class FloatingText
{
    public bool active;
    public TextMeshProUGUI txt;
    public GameObject go;
    public Vector3 motion;
    public float duration;
    public float lastShown;

    public void Show()
    {
        active = true;
        lastShown = Time.time;
        go.SetActive(true);
    }

    public void Hide()
    {
        active = false;
        go.SetActive(false);
    }

    public void UpdateFloatingText()
    {
        if (!active)
            return;

        if (Time.time - lastShown > duration)
        {
            Hide();
            return;
        }

        go.transform.position += motion * Time.deltaTime;
    }

    
}
