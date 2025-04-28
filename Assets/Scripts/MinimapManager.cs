using System;
using System.Collections.Generic;
using System.Linq;
using Dungeon;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    public List<string> VisitedRooms = new();

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
        VisitedRooms.Add(currentRoom.GameObject.name);

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
            var visitedVisible = VisitedRoomsVisible && VisitedRooms.Contains(room.GameObject.name);
            var fogEnabled = !visitedVisible && !adjacentVisible;

            fogController.SetFog(fogEnabled);
        }
    }
}