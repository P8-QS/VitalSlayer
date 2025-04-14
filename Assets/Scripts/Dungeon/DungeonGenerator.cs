using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dungeon
{
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The grid component.")]
        public Grid grid;
        
        [Tooltip("Optional. Will be populated at runtime.")]
        public Transform generatedRoomsParent;

        [Header("Room Prefabs")]
        public GameObject startRoomPrefab;
        public List<GameObject> roomPrefabs;

        [Header("Generation Settings")]
        public int maxRooms = 15;
        public int maxAttemptsPerDoor = 10;
        public float doorPositionMatchTolerance = 0.01f;

        private List<RoomInstance> placedRooms = new List<RoomInstance>();

        private class RoomInstance
        {
            public GameObject GameObject;
            public Room RoomScript;
            public Bounds Bounds;
            public List<Transform> AvailableDoors;
            
            public RoomInstance(GameObject go, Room room, Bounds b)
            {
                GameObject = go;
                RoomScript = room;
                Bounds = b;
                AvailableDoors = new List<Transform>(room.GetDoorTransforms());
            }
        }

        public void GenerateDungeon()
        {
            ClearDungeon();

            if (grid is null || startRoomPrefab is null || roomPrefabs.Count == 0)
            {
                Debug.LogError("Dungeon Generator is not properly configured!");
                return;
            }
            // Ensure the Room script has the necessary data structure
            if (startRoomPrefab.GetComponent<Room>()?.GetDoorPrefabData() is null) {
                Debug.LogError($"Start Room Prefab '{startRoomPrefab.name}' is missing Room script or GetDoorPrefabData() method is not working correctly. Ensure Room.cs is set up for Method 2.");
                return;
            }
            if (roomPrefabs.Any(p => p?.GetComponent<Room>()?.GetDoorPrefabData() is null)) {
                Debug.LogError($"One or more Room Prefabs are null, missing the Room script, or GetDoorPrefabData() is not working correctly. Ensure Room.cs is set up for Method 2 and all prefabs are assigned.");
                return;
            }
            
            if (generatedRoomsParent is null)
            {
                generatedRoomsParent = grid.transform;
            }

            // Place start room
            if (!TryPlaceRoom(startRoomPrefab, Vector3Int.zero))
            {
                Debug.LogError("Failed to place the starting room!");
                return;
            }
        
            var roomsToProcess = new Queue<RoomInstance>();
            roomsToProcess.Enqueue(placedRooms[0]);

            var roomsPlacedCount = 1;
            while (roomsToProcess.Count > 0 && roomsPlacedCount < maxRooms)
            {
                var currentRoomInstance = roomsToProcess.Dequeue();

                // Shuffle actual door Transforms for randomness before processing
                var shuffledDoorTransforms = currentRoomInstance.AvailableDoors.OrderBy(d => Random.value).ToList();
                var numberOfDoors = Random.Range(1, shuffledDoorTransforms.Count+1);

                var doorsPopulatedCount = 0;
                foreach (var currentDoorTransform in shuffledDoorTransforms)
                {
                    if (doorsPopulatedCount >= numberOfDoors) continue;
                    if (!currentRoomInstance.AvailableDoors.Contains(currentDoorTransform)) continue;
                
                    if (roomsPlacedCount >= maxRooms) break;

                    var currentDoorDir = Room.GetDoorDirection(currentDoorTransform);
                    var requiredOppositeDir = Room.GetOppositeDirection(currentDoorDir);
                    if (requiredOppositeDir == null)
                    {
                        Debug.LogWarning($"Door {currentDoorTransform.name} on {currentRoomInstance.GameObject.name} has invalid direction format.", currentDoorTransform);
                        continue; // Invalid door name/format
                    }

                    for (var attempt = 0; attempt < maxAttemptsPerDoor; attempt++)
                    {
                        var nextRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
                        var nextRoomScript = nextRoomPrefab.GetComponent<Room>();

                        if (nextRoomScript is null)
                        {
                            Debug.LogError($"Prefab {nextRoomPrefab.name} is missing Room script! Skipping.", nextRoomPrefab);
                            continue;
                        }
                    
                        var potentialNewDoorsData = nextRoomScript.GetDoorPrefabData()
                            .Where(doorInfo => doorInfo.direction == requiredOppositeDir)
                            .OrderBy(d => Random.value) // Shuffle potential matches
                            .ToList();

                        if (potentialNewDoorsData.Count == 0)
                        {
                            Debug.Log($"Prefab {nextRoomPrefab.name} has no door data for direction {requiredOppositeDir}");
                            continue;
                        }
                    
                        var chosenDoorData = potentialNewDoorsData[0];
                        var currentDoorWorldPos = currentDoorTransform.position;
                        var newRoomDoorLocalPos = chosenDoorData.localPosition;

                        var targetNewRoomWorldPos = currentDoorWorldPos - newRoomDoorLocalPos;
                        var targetNewRoomGridPos = grid.WorldToCell(targetNewRoomWorldPos);

                        if (!TryPlaceRoom(nextRoomPrefab, targetNewRoomGridPos)) continue;
                    
                        doorsPopulatedCount++;
                        roomsPlacedCount++;
                        var newRoomInstance = placedRooms[^1];
                        roomsToProcess.Enqueue(newRoomInstance);
                        
                        var placedDoorInstance = newRoomInstance.RoomScript.GetDoorTransforms()
                            .FirstOrDefault(t => Room.GetDoorDirection(t) == requiredOppositeDir &&
                                                 Vector3.Distance(t.localPosition, chosenDoorData.localPosition) < doorPositionMatchTolerance);

                        if (placedDoorInstance is not null) {
                            newRoomInstance.AvailableDoors.Remove(placedDoorInstance);
                        } else {
                            // This might happen if DoorInfo data is stale or tolerance is too small
                            Debug.LogWarning($"Could not find corresponding door transform on newly placed room {newRoomInstance.GameObject.name} matching data (Dir: {requiredOppositeDir}, LocalPos: {chosenDoorData.localPosition}). Check Room prefab's Door Data and doorPositionMatchTolerance.", newRoomInstance.GameObject);
                        }

                        currentRoomInstance.AvailableDoors.Remove(currentDoorTransform);
                        break;
                    }
                    // If after all attempts, no room was placed for 'currentDoorTransform', it remains in 'availableDoors'.
                }
            }

            Debug.Log($"Dungeon generation complete. Placed {placedRooms.Count} rooms.");
            // Optional: Post-processing
        }

        // --- Helper Functions ---
        private bool TryPlaceRoom(GameObject roomPrefab, Vector3Int gridPosition)
        {
            var roomPrefabScript = roomPrefab.GetComponent<Room>();
            if (roomPrefabScript is null)
            {
                Debug.LogError($"Prefab {roomPrefab.name} is missing Room script!", roomPrefab);
                return false;
            }

            var potentialBounds = roomPrefabScript.GetRoomBounds();
            var potentialPosition = grid.GetCellCenterWorld(gridPosition);
            potentialBounds.center += potentialPosition;

            if (placedRooms.Any(existingRoom => BoundsOverlap(potentialBounds, existingRoom.Bounds)))
            {
                Debug.LogWarning("Detected overlap");
                return false; // Potential overlap detected TODO: uncomment once map is fixed
            }

            // No collisions detected, safe to instantiate
            var roomInstanceGo = Instantiate(roomPrefab, generatedRoomsParent, true);
            roomInstanceGo.name = $"{roomPrefab.name}_({potentialBounds.center.x},{potentialBounds.center.y})";
            roomInstanceGo.transform.position = potentialPosition;
            var roomScript = roomInstanceGo.GetComponent<Room>();
            roomScript.InvalidateBoundsCache();
        
            var newInstance = new RoomInstance(roomInstanceGo, roomScript, potentialBounds);
            placedRooms.Add(newInstance);

            return true;
        }
    
        bool BoundsOverlap(Bounds a, Bounds b)
        {
            return !Mathf.Approximately(a.max.x, b.min.x) && a.max.x > b.min.x &&
                   !Mathf.Approximately(b.max.x, a.min.x) && b.max.x > a.min.x &&
                   !Mathf.Approximately(a.max.y, b.min.y) && a.max.y > b.min.y &&
                   !Mathf.Approximately(b.max.y, a.min.y) && b.max.y > a.min.y;
        }

        public void ClearDungeon()
        {
            placedRooms.Clear();
            var parentToClear = generatedRoomsParent ?? grid.transform;

            for (var i = parentToClear.childCount - 1; i >= 0; i--)
            {
                var childGo = parentToClear.GetChild(i).gameObject;
                if (childGo.GetComponent<Room>() is null) continue; // Identify generated rooms
                if (Application.isPlaying)
                    Destroy(childGo);
                else
                    DestroyImmediate(childGo);
            }
            switch (Application.isPlaying)
            {
                case false when parentToClear.childCount == 0:
                    Debug.Log("Cleared previous dungeon (Editor).");
                    break;
                case false when parentToClear.childCount > 0:
                    Debug.LogWarning("Clear Dungeon finished, but some children remained (maybe not rooms?).");
                    break;
            }
        }
    }
}