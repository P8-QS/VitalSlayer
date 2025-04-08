using UnityEngine;

public class RoomFogController : MonoBehaviour
{
    public GameObject fogOverlay; // Assign in inspector or find via script

    public void RevealFog()
    {
        if (fogOverlay != null)
            fogOverlay.SetActive(false);
    }

    public void HideFog()
    {
        if (fogOverlay != null)
            fogOverlay.SetActive(true);
    }
}
