using UnityEngine;
public class ModalUI : MonoBehaviour
{
    public GameObject content;

    public void SetContent(GameObject content)
    {
        this.content = content;
    }
    public void OnBlur()
    {
        OnClose();
    }
    public void OnClose() {
        Destroy(gameObject);
    }
}
