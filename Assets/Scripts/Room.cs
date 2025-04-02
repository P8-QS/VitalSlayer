using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [System.Serializable]
    public enum DoorOrientation
    {
        Front,
        Side
    }

    [System.Serializable]
    public class DoorData
    {
        public Vector3Int position;
        public DoorOrientation orientation;
    }

    [Header("Room Boundaries")]
    [Tooltip("First corner of the room (usually the bottom-left)")]
    public Vector2 boundaryCorner1;

    [Tooltip("Second corner of the room (usually the top-right)")]
    public Vector2 boundaryCorner2;

    [Header("Door Settings")]
    [Tooltip("Reference to the Tilemap that contains the door tiles")]
    public Tilemap doorTilemap;

    [Tooltip("The tile to use for closed front-facing doors")]
    public TileBase closedFrontDoorTile;

    [Tooltip("The tile to use for open front-facing doors")]
    public TileBase openFrontDoorTile;

    [Tooltip("The tile to use for closed side-facing doors")]
    public TileBase closedSideDoorTile;

    [Tooltip("The tile to use for open side-facing doors")]
    public TileBase openSideDoorTile;

    [Tooltip("List of door positions and their orientations")]
    public List<DoorData> doors = new List<DoorData>();

    [Header("Enemy Settings")]
    [Tooltip("Tag used to identify enemies that need to be defeated")]
    public string enemyTag = "Fighter";

    // Calculated bounds
    private Bounds roomBounds;

    // List of enemies that belong to this room
    private List<GameObject> roomEnemies = new List<GameObject>();
    private bool doorsOpened = false;

    // Show the room boundaries in the editor
    private void OnDrawGizmos()
    {
        // Calculate the bounds from the two corners
        Vector3 min = new Vector3(
            Mathf.Min(boundaryCorner1.x, boundaryCorner2.x),
            Mathf.Min(boundaryCorner1.y, boundaryCorner2.y),
            0
        );

        Vector3 max = new Vector3(
            Mathf.Max(boundaryCorner1.x, boundaryCorner2.x),
            Mathf.Max(boundaryCorner1.y, boundaryCorner2.y),
            0
        );

        Vector3 center = (min + max) * 0.5f;
        Vector3 size = max - min;

        // Draw the room boundary
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, size);

        // Draw door positions
        if (doorTilemap != null)
        {
            foreach (DoorData door in doors)
            {
                Vector3 worldPos = doorTilemap.CellToWorld(door.position);
                // Add cell size offset to center the marker in the cell
                worldPos += (Vector3)doorTilemap.cellSize * 0.5f;
                // Add a small offset to make sure the door marker is visible
                worldPos.z = -0.1f;

                // Use different colors for different door orientations
                Gizmos.color = door.orientation == DoorOrientation.Front ? Color.yellow : Color.green;
                Gizmos.DrawWireSphere(worldPos, 0.2f);
            }
        }
    }

    void Start()
    {
        // Calculate the bounds from the two corners
        CalculateRoomBounds();

        // Make sure we have the necessary components
        if (doorTilemap == null)
        {
            Debug.LogError("Door tilemap is not assigned!");
            return;
        }

        // Check if the door tiles are assigned
        if (closedFrontDoorTile == null || openFrontDoorTile == null)
        {
            Debug.LogError("Front door tiles are not assigned!");
            return;
        }

        if (closedSideDoorTile == null || openSideDoorTile == null)
        {
            Debug.LogError("Side door tiles are not assigned!");
            return;
        }

        // Set all doors to closed initially
        foreach (DoorData door in doors)
        {
            // Set the appropriate closed door tile based on orientation
            TileBase closedTile = door.orientation == DoorOrientation.Front
                ? closedFrontDoorTile
                : closedSideDoorTile;

            doorTilemap.SetTile(door.position, closedTile);
        }

        // Find all enemies within this room's boundaries
        FindEnemiesInRoom();

        Debug.Log($"Room contains {roomEnemies.Count} enemies");
    }

    void Update()
    {
        // If doors are already open, no need to check enemies
        if (doorsOpened)
            return;

        // Check if all enemies are defeated
        bool allDefeated = true;

        // Iterate through enemies to see if any are still alive
        for (int i = roomEnemies.Count - 1; i >= 0; i--)
        {
            if (roomEnemies[i] != null)
            {
                // Still have an enemy alive
                allDefeated = false;
                break;
            }
            else
            {
                // Remove destroyed enemies from our list
                roomEnemies.RemoveAt(i);
            }
        }

        // If all enemies are defeated and we had enemies to begin with, open the doors
        if (allDefeated && roomEnemies.Count == 0)
        {
            OpenDoors();
        }
    }

    private void CalculateRoomBounds()
    {
        // Calculate the bounds from the two corners
        Vector3 min = new Vector3(
            Mathf.Min(boundaryCorner1.x, boundaryCorner2.x),
            Mathf.Min(boundaryCorner1.y, boundaryCorner2.y),
            0
        );

        Vector3 max = new Vector3(
            Mathf.Max(boundaryCorner1.x, boundaryCorner2.x),
            Mathf.Max(boundaryCorner1.y, boundaryCorner2.y),
            0
        );

        Vector3 center = (min + max) * 0.5f;
        Vector3 size = max - min;

        roomBounds = new Bounds(center, size);
    }

    private void FindEnemiesInRoom()
    {
        // Clear the current list
        roomEnemies.Clear();

        // Make sure we have valid bounds
        CalculateRoomBounds();

        // Find all objects with the specified tag
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(enemyTag);

        // Filter to only include enemies (tag=Fighter and name!=Player) within this room's boundaries
        foreach (GameObject obj in taggedObjects)
        {
            if (obj.name != "Player" && roomBounds.Contains(obj.transform.position))
            {
                roomEnemies.Add(obj);
            }
        }
    }

    public void OpenDoors()
    {
        if (doorsOpened)
            return;

        Debug.Log("Opening all doors in the room!");

        // Change all door tiles to open state based on their orientation
        foreach (DoorData door in doors)
        {
            TileBase openTile = door.orientation == DoorOrientation.Front
                ? openFrontDoorTile
                : openSideDoorTile;

            doorTilemap.SetTile(door.position, openTile);
        }

        // If using a composite collider, update it
        if (doorTilemap.TryGetComponent<TilemapCollider2D>(out var collider))
        {
            collider.ProcessTilemapChanges();
        }

        doorsOpened = true;
    }

    // Helper method to add a door at a specific position with an orientation
    public void AddDoor(Vector3Int tilePosition, DoorOrientation orientation)
    {
        // Check if a door already exists at this position
        bool doorExists = false;
        foreach (DoorData door in doors)
        {
            if (door.position == tilePosition)
            {
                doorExists = true;
                door.orientation = orientation; // Update the orientation
                break;
            }
        }

        // If no door exists at this position, add a new one
        if (!doorExists)
        {
            DoorData newDoor = new DoorData
            {
                position = tilePosition,
                orientation = orientation
            };

            doors.Add(newDoor);
        }

        // If the game is already running, set the tile to closed
        if (Application.isPlaying && doorTilemap != null)
        {
            TileBase closedTile = orientation == DoorOrientation.Front
                ? closedFrontDoorTile
                : closedSideDoorTile;

            doorTilemap.SetTile(tilePosition, closedTile);
        }
    }

    // Helper method to remove a door
    public void RemoveDoor(Vector3Int tilePosition)
    {
        for (int i = 0; i < doors.Count; i++)
        {
            if (doors[i].position == tilePosition)
            {
                doors.RemoveAt(i);
                return;
            }
        }
    }
}