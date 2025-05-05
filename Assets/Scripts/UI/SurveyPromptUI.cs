using TMPro;
using UnityEngine;

public class SurveyPromptUI : MonoBehaviour
{
    public GameObject inputObject;


    public void OnSubmit()
    {
        var email = inputObject.GetComponent<TMP_InputField>().text;

        if (string.IsNullOrEmpty(email))
        {
            Debug.Log("Email is empty");
            return;
        }

        StateManager.Instance.SetEmail(email);
        StateManager.Instance.SaveState();
        LoggingManager.Instance.LogEmail(email);
        Destroy(gameObject);
    }

    public void OnBlur()
    {
        StateManager.Instance.SetEmail("none");
        StateManager.Instance.SaveState();
        Destroy(gameObject);
    }
}