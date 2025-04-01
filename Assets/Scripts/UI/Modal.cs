using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ModalUI : MonoBehaviour
{
    public GameObject content;

    public void SetContent(GameObject content)
    {
        this.content = content;
    }
    
    public void OnClose() {
        Debug.Log("CLOSING MODAL!");
        Destroy(gameObject);
    }
}
