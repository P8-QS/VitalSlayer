using Dungeon;
using UnityEngine;

public class RoomFogController : MonoBehaviour
{
    public GameObject fogOverlay; // Assign in inspector or find via script

    public void SetFog(bool active)
    {
        if (fogOverlay != null)
            fogOverlay.SetActive(active);

        var minimapGfx = transform.Find("Minimap GFX")?.gameObject;
        if (!minimapGfx) return;
        minimapGfx.SetActive(!active);

        // Get enemies in room
        var room = gameObject.GetComponent<Room>();
        room.FindEnemiesInRoom();
        var enemies = room.RoomEnemies;

        Debug.Log("Found " + enemies.Count + " enemies in room: " + gameObject.name);
        
        foreach (var enemy in enemies)
        {
            var enemyFog = enemy.transform.Find("Minimap GFX")?.gameObject;
            if (enemyFog != null)
            {
                enemyFog.SetActive(!active);
            }
        }
    }
}