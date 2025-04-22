using System.Collections.Generic;
using System.Linq;
using Dungeon;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MinimapManager : MonoBehaviour
{
    public List<RoomInstance> VisitedRooms = new();

    public static MinimapManager Instance;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public bool AdjacentRoomsVisible = false;
    public bool VisitedRoomsVisible = false;


    public List<RoomInstance> GetRooms()
    {
        var dungeonGenerator = FindFirstObjectByType<DungeonGenerator>();
        var rooms = dungeonGenerator.PlacedRooms;
        return rooms;
    }

    public void UpdateMinimap(RoomInstance currentRoom)
    {
        VisitedRooms.Add(currentRoom);

        var rooms = GetRooms();
        Debug.Log("Updating minimap total rooms: " + rooms.Count);
        Debug.Log("Updating minimap current room: " + currentRoom.RoomScript.gameObject.name);

        foreach (var room in rooms)
        {
            var fogController = room.GameObject.GetComponent<RoomFogController>();

            if (fogController == null)
            {
                Debug.LogWarning("No RoomFogController found on room: " + room.GameObject.name);
                continue;
            }
            ;

            Debug.Log("Updating minimap for room fog: " + room.RoomScript.gameObject.name);

            if (room.GameObject.name == currentRoom.GameObject.name)
            {
                Debug.LogWarning("Room " + currentRoom.RoomScript.gameObject.name + " is currently in use.");
                fogController.SetFog(false);
                continue;
            }

            var isAdjacent = room.RoomScript.connectedDoors
                .Any(door => door.RoomB == currentRoom.RoomScript || door.RoomA == currentRoom.RoomScript);

            var adjacentVisible = AdjacentRoomsVisible && isAdjacent;
            var visitedVisible = VisitedRoomsVisible && VisitedRooms.Contains(room);
            var fogEnabled = !visitedVisible && !adjacentVisible;

            Debug.Log("Setting fog to " + fogEnabled + " for room: " + room.RoomScript.gameObject.name);

            fogController.SetFog(fogEnabled);
        }
    }
}