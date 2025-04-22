using UnityEngine;

public class RoomFogController : MonoBehaviour
{
    public GameObject fogOverlay; // Assign in inspector or find via script

    public void SetFog(bool active)
    {
        if (fogOverlay != null)
            fogOverlay.SetActive(active);

        var minimapGfx = GameObject.Find("Minimap GFX");
        if (!minimapGfx) return;

        minimapGfx.SetActive(active);
    }
}