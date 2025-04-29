using System;
using System.Collections.Generic;
using System.Linq;
using Dungeon;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    public List<string> VisitedRooms = new();

    public static MinimapManager Instance;

    public float visibleAlpha = 0.95f;
    public float invisibleAlpha = 0.2f;

    private float currentAlpha;
    public float fadeSpeed = 5f; // Adjust for faster/slower fade

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


    public void Update()
    {
        var player = GameManager.Instance.player;

        if (!Camera.main || !player) return;

        var screenPoint = Camera.main.WorldToScreenPoint(player.transform.position);
        var minimapRect = gameObject.GetComponent<RectTransform>();

        // Get screen-space corners of the minimap UI
        Vector3[] corners = new Vector3[4];
        minimapRect.GetWorldCorners(corners);

        Rect minimapScreenRect = new Rect(corners[0].x, corners[0].y,
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y);

        bool playerOverlap = minimapScreenRect.Contains(screenPoint);

        var canvas = gameObject.GetComponent<CanvasGroup>();

        float targetAlpha = playerOverlap ? invisibleAlpha : visibleAlpha;

        // Smoothly interpolate the alpha
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        canvas.alpha = currentAlpha;
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
            }

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