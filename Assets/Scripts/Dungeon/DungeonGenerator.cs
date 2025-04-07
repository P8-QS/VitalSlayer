using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    [Header("References")]
    public Grid grid;
    public Transform generatedRoomsParent; // Optional

    [Header("Room Prefabs")]
    public GameObject startRoomPrefab;
    public List<GameObject> roomPrefabs;

    [Header("Generation Settings")]
    public int maxRooms = 15;
    public int maxAttemptsPerDoor = 10;
    public float doorPositionMatchTolerance = 0.01f;

    // --- Private runtime data ---
    private List<RoomInstance> placedRooms = new List<RoomInstance>();

    // Helper class to store info about instantiated rooms
    private class RoomInstance
    {
        public GameObject gameObject;
        public Room roomScript;
        public Bounds bounds; // Bounds in Grid cell coordinates
        public List<Transform> availableDoors; // Actual Transform instances from the placed room

        public RoomInstance(GameObject go, Room room, Bounds b)
        {
            gameObject = go;
            roomScript = room;
            bounds = b;
            // Get the actual door Transforms from the instantiated room's script at runtime
            availableDoors = new List<Transform>(room.GetDoorTransforms());
        }
    }

    // --- Generation Trigger ---
    [ContextMenu("Generate Dungeon (Using Prefab Data)")] // Renamed for clarity
    public void GenerateDungeon()
    {
        ClearDungeon();

        if (grid == null || startRoomPrefab == null || roomPrefabs.Count == 0)
        {
            Debug.LogError("Dungeon Generator is not properly configured!");
            return;
        }
         // Ensure the Room script has the necessary data structure (runtime check)
         if (startRoomPrefab.GetComponent<Room>() == null || startRoomPrefab.GetComponent<Room>().GetDoorPrefabData() == null) {
              Debug.LogError($"Start Room Prefab '{startRoomPrefab.name}' is missing Room script or GetDoorPrefabData() method is not working correctly. Ensure Room.cs is set up for Method 2.");
              return;
         }
         if (roomPrefabs.Any(p => p == null || p.GetComponent<Room>() == null || p.GetComponent<Room>().GetDoorPrefabData() == null)) {
             Debug.LogError($"One or more Room Prefabs are null, missing the Room script, or GetDoorPrefabData() is not working correctly. Ensure Room.cs is set up for Method 2 and all prefabs are assigned.");
             return;
         }


        if (generatedRoomsParent == null)
        {
            generatedRoomsParent = grid.transform; // Default to grid itself
        }

        // 1. Place Start Room
        if (!TryPlaceRoom(startRoomPrefab, Vector3Int.zero, null, null)) // No parent room/door for start room
        {
            Debug.LogError("Failed to place the starting room!");
            return;
        }

        // 2. Generation Loop using a Queue (Breadth-First Expansion)
        Queue<RoomInstance> roomsToProcess = new Queue<RoomInstance>();
        roomsToProcess.Enqueue(placedRooms[0]); // Enqueue the start room instance

        int roomsPlacedCount = 1;

        while (roomsToProcess.Count > 0 && roomsPlacedCount < maxRooms)
        {
            RoomInstance currentRoomInstance = roomsToProcess.Dequeue();

            // Shuffle actual door Transforms for randomness before processing
            List<Transform> shuffledDoorTransforms = currentRoomInstance.availableDoors.OrderBy(d => Random.value).ToList();

            foreach (Transform currentDoorTransform in shuffledDoorTransforms) // Iterate through the available runtime Transforms
            {
                 // Check availableDoors again in case it was connected by another path
                 if (!currentRoomInstance.availableDoors.Contains(currentDoorTransform)) continue;

                if (roomsPlacedCount >= maxRooms) break; // Stop if max rooms reached

                string currentDoorDir = Room.GetDoorDirection(currentDoorTransform);
                string requiredOppositeDir = Room.GetOppositeDirection(currentDoorDir);
                if (requiredOppositeDir == null)
                {
                    Debug.LogWarning($"Door {currentDoorTransform.name} on {currentRoomInstance.gameObject.name} has invalid direction format.", currentDoorTransform);
                    continue; // Invalid door name/format
                }

                bool roomPlacedForThisDoor = false;
                for (int attempt = 0; attempt < maxAttemptsPerDoor; attempt++)
                {
                    // --- Step 1: Select Prefab and Get its Room Component ---
                    GameObject nextRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
                    Room nextRoomScriptOnPrefab = nextRoomPrefab.GetComponent<Room>(); // Get component directly from prefab

                    if (nextRoomScriptOnPrefab == null)
                    {
                        Debug.LogError($"Prefab {nextRoomPrefab.name} is missing Room script! Skipping.", nextRoomPrefab);
                        continue; // Try next attempt with a different prefab
                    }

                    // --- Step 2: Find Compatible Door DATA on the Prefab ---
                    // Access the pre-populated door data list from the prefab's component
                    List<Room.DoorInfo> potentialNewDoorsData = nextRoomScriptOnPrefab.GetDoorPrefabData()
                        .Where(doorInfo => doorInfo.direction == requiredOppositeDir)
                        .OrderBy(d => Random.value) // Shuffle potential matches
                        .ToList();

                    // --- Step 3: Check if suitable door data exists ---
                    if (potentialNewDoorsData.Count == 0)
                    {
                        // This prefab doesn't have pre-defined door data facing the required direction
                        // Or the doorData list might be empty/not populated on the prefab!
                         // Debug.Log($"Prefab {nextRoomPrefab.name} has no door data for direction {requiredOppositeDir}");
                        continue; // Try next attempt (potentially with a different prefab)
                    }

                    // --- Step 4: Calculate Target Position using Door Data ---
                    // A suitable door DATA entry was found. Use its info.
                    Room.DoorInfo chosenDoorData = potentialNewDoorsData[0];

                    // World position of the door we are branching FROM (on the already placed room)
                    Vector3 currentDoorWorldPos = currentDoorTransform.position;
                    // Local position of the chosen door WITHIN the new room structure (from the prefab data)
                    Vector3 newRoomDoorLocalPos = chosenDoorData.localPosition;

                    // Target world position for the *new* room's origin (pivot)
                    Vector3 targetNewRoomWorldPos = currentDoorWorldPos - newRoomDoorLocalPos;
                    Vector3Int targetNewRoomGridPos = grid.WorldToCell(targetNewRoomWorldPos);

                    // --- Step 5: Try Placing the Selected Prefab ---
                    // No temporary instantiation needed here for checking. Directly try placement.
                    if (TryPlaceRoom(nextRoomPrefab, targetNewRoomGridPos, currentRoomInstance, currentDoorTransform))
                    {
                        roomsPlacedCount++;
                        RoomInstance newRoomInstance = placedRooms[placedRooms.Count - 1];
                        roomsToProcess.Enqueue(newRoomInstance); // Add the new room to the queue

                        // --- Step 6: Mark Doors as Connected (Matching Transform to Data) ---
                        // Find the actual Transform instance on the *placed* room that corresponds
                        // to the 'chosenDoorData' we used for placement.
                         Transform placedDoorInstance = newRoomInstance.roomScript.GetDoorTransforms()
                             .FirstOrDefault(t => Room.GetDoorDirection(t) == requiredOppositeDir &&
                                           Vector3.Distance(t.localPosition, chosenDoorData.localPosition) < doorPositionMatchTolerance); // Match using direction and local pos

                         if (placedDoorInstance != null) {
                              // Remove the corresponding Transform from the *newly placed room's* available list
                              newRoomInstance.availableDoors.Remove(placedDoorInstance);
                              // Debug.Log($"Removed connection door {placedDoorInstance.name} from newly placed room {newRoomInstance.gameObject.name}");
                         } else {
                             // This might happen if DoorInfo data is stale or tolerance is too small
                             Debug.LogWarning($"Could not find corresponding door transform on newly placed room {newRoomInstance.gameObject.name} matching data (Dir: {requiredOppositeDir}, LocalPos: {chosenDoorData.localPosition}). Check Room prefab's Door Data and doorPositionMatchTolerance.", newRoomInstance.gameObject);
                         }

                         // Remove the door Transform we just branched FROM from the *current* room's available list
                         currentRoomInstance.availableDoors.Remove(currentDoorTransform);
                         // Debug.Log($"Removed originating door {currentDoorTransform.name} from room {currentRoomInstance.gameObject.name}");

                        roomPlacedForThisDoor = true;
                        break; // Exit attempt loop for this door, move to the next available door on currentRoomInstance
                    }
                    // else: TryPlaceRoom failed (e.g., overlap). Loop continues to the next attempt for this *same* 'currentDoorTransform'.
                }
                 // If after all attempts, no room was placed for 'currentDoorTransform', it remains in 'availableDoors'.
            }
        }

        Debug.Log($"Dungeon generation complete. Placed {placedRooms.Count} rooms.");
        // Optional: Post-processing
    }

    // --- Helper Functions ---
    private bool TryPlaceRoom(GameObject roomPrefab, Vector3Int gridPosition, RoomInstance parentRoom, Transform parentDoor)
    {
        var roomPrefabScript = roomPrefab.GetComponent<Room>();
        if (roomPrefabScript == null)
        {
            Debug.LogError($"Prefab {roomPrefab.name} is missing Room script!", roomPrefab);
            return false;
        }

        var potentialBounds = roomPrefabScript.GetRoomBounds();
        var potentialPosition = grid.GetCellCenterWorld(gridPosition);
        potentialBounds.center += potentialPosition;

        if (placedRooms.Any(existingRoom => BoundsOverlap(potentialBounds, existingRoom.bounds)))
        {
            return false; // Potential overlap detected
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

    [ContextMenu("Clear Dungeon")]
    public void ClearDungeon()
    {
        placedRooms.Clear();
        var parentToClear = (generatedRoomsParent != null) ? generatedRoomsParent : grid.transform;

        for (var i = parentToClear.childCount - 1; i >= 0; i--)
        {
            var childGo = parentToClear.GetChild(i).gameObject;
            if (childGo.GetComponent<Room>() == null) continue; // Identify generated rooms
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