using UnityEngine;
using UnityEngine.UI;

public class ResetPointsButton : MonoBehaviour
{
    public Button resetButton;

    void Start()
    {
        CheckIfButtonShouldBeActive();
    }

    void Update()
    {
        CheckIfButtonShouldBeActive();
    }

    void CheckIfButtonShouldBeActive()
    {
        if (ExperienceManager.Instance.Level <= 1 || PerksManager.Instance.ActivePerkCount() == 0)
        {
            resetButton.interactable = false;
        }
        else
        {
            resetButton.interactable = true;
        }
    }
}
