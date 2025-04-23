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

        foreach (var room in rooms)
        {
            var fogController = room.GameObject.GetComponent<RoomFogController>();
            
            if (fogController == null)
            {
                continue;
            };

            if (room.GameObject.name == currentRoom.GameObject.name)
            {
                fogController.SetFog(false);
                continue;
            }
            
            var isAdjacent = room.RoomScript.connectedDoors
                .Any(door => door.RoomB == currentRoom.RoomScript || door.RoomA == currentRoom.RoomScript);
            
            var adjacentVisible = AdjacentRoomsVisible && isAdjacent;
            var visitedVisible = VisitedRoomsVisible && VisitedRooms.Contains(room);
            var fogEnabled = !visitedVisible && !adjacentVisible;

            fogController.SetFog(fogEnabled);
        }
    }
}