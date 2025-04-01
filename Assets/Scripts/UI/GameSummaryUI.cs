using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSummaryUI : MonoBehaviour
{
    public GameObject summaryItemPrefab;
    public TextMeshProUGUI summaryTitleText;
    public Transform parentPanel; 
    public bool gameWon;
    public Sprite gameWonBackground;
    public Sprite gameLostBackground;

    void Start()
    {   
        if (gameWon)
        {
            GetComponent<Image>().sprite = gameWonBackground;
            summaryTitleText.text = "You Won!";
        }
        else
        {
            GetComponent<Image>().sprite = gameLostBackground;
            summaryTitleText.text = "You Died!";

        }
    }

    public void AddSummaryItem(string name, string value)
    {
        GameObject summaryItem = Instantiate(summaryItemPrefab, parentPanel);
        SummaryItem summaryItemScript = summaryItem.GetComponent<SummaryItem>();
        summaryItemScript.SetValue(name, value);
    }
    
    public void OnButtonClick() {
        SceneLoader.Instance.LoadSceneWithTransition("MainMenu");
    }
    
        
}
