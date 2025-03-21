using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;


// Predefined room sizes, lets say 3 different types. 20x20, 30x30, 40x40
public class DungeonFloorGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct RoomSize
    {
        public int width;
        public int height;
    }

    [Header("Dungeon Parameters")]
    public int roomCountMin = 5;
    public int roomCountMax = 10;
    [Tooltip("List of predefined room sizes to use randomly")]
    public List<RoomSize> predefinedRoomSizes = new List<RoomSize>();
    public int roomSpacing = 0;  // Spacing between rooms (0 for directly adjacent)

    [Header("Tiles")]
    public RuleTile floorRuleTile;
    public RuleTile wallRuleTile;

    [Header("Tilemaps")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    private Dictionary<Vector2Int, int> dungeonGrid = new Dictionary<Vector2Int, int>(); // 0 = empty, 1 = floor, 2 = wall, 3 = floor and wall
    private List<Vector2Int> roomCenters = new List<Vector2Int>();
    
    // Track bounds for optimization
    private int minX = int.MaxValue;
    private int minY = int.MaxValue;
    private int maxX = int.MinValue;
    private int maxY = int.MinValue;
    
    // Cache the maximum room dimensions
    private int maxRoomWidth = 0;
    private int maxRoomHeight = 0;

    private void OnValidate()
    {
        // Ensure we have at least one room size
        if (predefinedRoomSizes.Count == 0)
        {
            predefinedRoomSizes.Add(new RoomSize { width = 20, height = 20 });
        }
        
        // Update max dimensions
        UpdateMaxRoomDimensions();
    }
    
    private void UpdateMaxRoomDimensions()
    {
        maxRoomWidth = 0;
        maxRoomHeight = 0;
        
        foreach (var roomSize in predefinedRoomSizes)
        {
            maxRoomWidth = Mathf.Max(maxRoomWidth, roomSize.width);
            maxRoomHeight = Mathf.Max(maxRoomHeight, roomSize.height);
        }
    }

    public void GenerateDungeon()
    {
        // Ensure max dimensions are updated
        UpdateMaxRoomDimensions();
        
        // Clear previous data
        dungeonGrid.Clear();
        roomCenters.Clear();
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        
        // Reset bounds
        minX = int.MaxValue;
        minY = int.MaxValue;
        maxX = int.MinValue;
        maxY = int.MinValue;
        
        // Generate rooms
        int roomCount = Random.Range(roomCountMin, roomCountMax + 1);
        GenerateRoom(roomCount);
        
        // Place walls around floors
        PlaceWalls();
        
        // Instantiate tiles based on the grid
        InstantiateTiles();
    }

    private void GenerateRoom(int roomCount)
    {
        // Start with a room at (0,0)
        int startX = 0;
        int startY = 0;
        CreateRoom(startX, startY);
        roomCenters.Add(new Vector2Int(startX, startY));
        
        // List of potential room positions
        List<Vector2Int> potentialRoomPositions = new List<Vector2Int>();
        
        // Add adjacent positions to the first room
        AddPotentialRoomPositions(startX, startY, potentialRoomPositions);
        
        // Create remaining rooms
        int roomsCreated = 1; // We already created one
        
        while (roomsCreated < roomCount && potentialRoomPositions.Count > 0)
        {
            // Pick a random position from potential positions
            int index = Random.Range(0, potentialRoomPositions.Count);
            Vector2Int pos = potentialRoomPositions[index];
            potentialRoomPositions.RemoveAt(index);
            
            // Check if we can place a room here (not overlapping with existing rooms)
            if (CanPlaceRoom(pos.x, pos.y))
            {
                // Create room
                CreateRoom(pos.x, pos.y);
                roomCenters.Add(pos);
                roomsCreated++;
                
                // Add new potential positions
                AddPotentialRoomPositions(pos.x, pos.y, potentialRoomPositions);
            }
        }
        
        // Connect adjacent rooms with corridors
        ConnectAdjacentRooms();
    }

    private RoomSize GetRandomRoomSize()
    {
        if (predefinedRoomSizes.Count == 0)
            return new RoomSize { width = 20, height = 20 }; // Default fallback
            
        int index = Random.Range(0, predefinedRoomSizes.Count);
        return predefinedRoomSizes[index];
    }

    private void CreateRoom(int centerX, int centerY)
    {
        RoomSize size = GetRandomRoomSize();
        int roomWidth = size.width;
        int roomHeight = size.height;
        int halfWidth = roomWidth / 2;
        int halfHeight = roomHeight / 2;
        
        // Fill the room with floor tiles
        for (int x = centerX - halfWidth; x <= centerX + halfWidth; x++)
        {
            for (int y = centerY - halfHeight; y <= centerY + halfHeight; y++)
            {
                dungeonGrid[new Vector2Int(x, y)] = 1; // floor
                
                // Update bounds
                minX = Mathf.Min(minX, x);
                minY = Mathf.Min(minY, y);
                maxX = Mathf.Max(maxX, x);
                maxY = Mathf.Max(maxY, y);
            }
        }
    }

    private bool CanPlaceRoom(int centerX, int centerY)
    {
        RoomSize size = GetRandomRoomSize();
        int roomWidth = size.width;
        int roomHeight = size.height;
        int halfWidth = roomWidth / 2;
        int halfHeight = roomHeight / 2;
        
        // Check if the room would overlap with existing rooms (with spacing)
        for (int x = centerX - halfWidth - roomSpacing; x <= centerX + halfWidth + roomSpacing; x++)
        {
            for (int y = centerY - halfHeight - roomSpacing; y <= centerY + halfHeight + roomSpacing; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (dungeonGrid.ContainsKey(pos) && dungeonGrid[pos] == 1) // If we find a floor tile
                {
                    return false; // Room would overlap
                }
            }
        }
        
        return true;
    }

    private void AddPotentialRoomPositions(int centerX, int centerY, List<Vector2Int> positions)
    {
        int cellSize = Mathf.Max(maxRoomWidth, maxRoomHeight) + roomSpacing;
        
        // Add the four adjacent positions
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, cellSize),   // Up
            new Vector2Int(0, -cellSize),  // Down
            new Vector2Int(cellSize, 0),   // Right
            new Vector2Int(-cellSize, 0)   // Left
        };
        
        foreach (Vector2Int dir in directions)
        {
            Vector2Int newPos = new Vector2Int(centerX + dir.x, centerY + dir.y);
            
            // Check if this position is already in our list
            bool alreadyExists = positions.Contains(newPos) || roomCenters.Contains(newPos);
            
            if (!alreadyExists)
            {
                positions.Add(newPos);
            }
        }
    }

    private void ConnectAdjacentRooms()
    {
        int cellSize = Mathf.Max(maxRoomWidth, maxRoomHeight) + roomSpacing;
        int corridorWidth = 6;
        int halfCorridorWidth = corridorWidth / 2;
        
        // For each room
        for (int i = 0; i < roomCenters.Count; i++)
        {
            Vector2Int room = roomCenters[i];
            
            // Check all other rooms
            for (int j = i + 1; j < roomCenters.Count; j++)
            {
                Vector2Int otherRoom = roomCenters[j];
                
                // If rooms are adjacent horizontally
                if (Mathf.Abs(room.x - otherRoom.x) == cellSize && room.y == otherRoom.y)
                {
                    // Create horizontal corridor
                    int minX = Mathf.Min(room.x, otherRoom.x);
                    int maxX = Mathf.Max(room.x, otherRoom.x);
                    for (int x = minX; x <= maxX; x++)
                    {
                        for (int y = room.y - halfCorridorWidth; y <= room.y + halfCorridorWidth; y++)
                        {
                            Vector2Int pos = new Vector2Int(x, y);
                            dungeonGrid[pos] = 1;
                            
                            // Update bounds
                            this.minX = Mathf.Min(this.minX, x);
                            this.minY = Mathf.Min(this.minY, y);
                            this.maxX = Mathf.Max(this.maxX, x);
                            this.maxY = Mathf.Max(this.maxY, y);
                        }
                    }
                }
                
                // If rooms are adjacent vertically
                if (Mathf.Abs(room.y - otherRoom.y) == cellSize && room.x == otherRoom.x)
                {
                    // Create vertical corridor
                    int minY = Mathf.Min(room.y, otherRoom.y);
                    int maxY = Mathf.Max(room.y, otherRoom.y);
                    for (int y = minY; y <= maxY; y++)
                    {
                        for (int x = room.x - halfCorridorWidth; x <= room.x + halfCorridorWidth; x++)
                        {
                            Vector2Int pos = new Vector2Int(x, y);
                            dungeonGrid[pos] = 1;
                            
                            // Update bounds
                            this.minX = Mathf.Min(this.minX, x);
                            this.minY = Mathf.Min(this.minY, y);
                            this.maxX = Mathf.Max(this.maxX, x);
                            this.maxY = Mathf.Max(this.maxY, y);
                        }
                    }
                }
            }
        }
    }

    private void PlaceWalls()
    {
        // Create a copy of all floor positions
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        foreach (var kvp in dungeonGrid)
        {
            if (kvp.Value == 1)
                floorPositions.Add(kvp.Key);
        }
        
        // Define adjacent positions (8 directions - cardinal directions and diagonals)
        int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
        int[] dy = { 1, 1, 0, -1, -1, -1, 0, 1 };
        
        // For each floor position
        foreach (Vector2Int pos in floorPositions)
        {
            // Check if this floor tile is on the edge (has at least one empty neighbor)
            bool isEdge = false;
            for (int i = 0; i < 8; i++)
            {
                Vector2Int neighborPos = new Vector2Int(pos.x + dx[i], pos.y + dy[i]);
                
                // Check if empty
                if (!dungeonGrid.ContainsKey(neighborPos) || dungeonGrid[neighborPos] == 0)
                {
                    isEdge = true;
                    break;
                }
            }
            
            // If this is an edge floor tile, also mark it as a wall
            if (isEdge)
            {
                dungeonGrid[pos] = 3; // Both floor and wall
            }
        }
    }

    private void InstantiateTiles()
    {
        // Only iterate through the bounds we've tracked
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                
                // Skip if no tile data at this position
                if (!dungeonGrid.ContainsKey(pos))
                    continue;
                
                if (dungeonGrid[pos] == 1 || dungeonGrid[pos] == 3) // floor or both
                {
                    floorTilemap.SetTile(tilePos, floorRuleTile);
                }
                
                if (dungeonGrid[pos] == 2 || dungeonGrid[pos] == 3) // wall or both
                {
                    wallTilemap.SetTile(tilePos, wallRuleTile);
                }
            }
        }
    }
}
