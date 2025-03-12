using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor; // If you're using custom tiles from the editor
using System.Collections.Generic;

public class DungeonFloorGenerator : MonoBehaviour
{
    [Header("Dungeon Parameters")]
    public int dungeonSize = 50;
    public int roomCountMin = 5;
    public int roomCountMax = 10;
    public int roomSize = 10;  // Fixed room size
    public int roomSpacing = 2;  // Spacing between rooms (0 for directly adjacent)

    [Header("Tiles")]
    public RuleTile floorRuleTile;  // Make sure to use UnityEngine.Tilemaps
    public RuleTile wallRuleTile;

    [Header("Tilemaps")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    private int[,] dungeonGrid; // 0 = empty, 1 = floor, 2 = wall

    // Store room positions for connecting them
    private List<Vector2Int> roomCenters = new List<Vector2Int>();

    public void GenerateDungeon()
    {
        // Initialize the grid
        dungeonGrid = new int[dungeonSize, dungeonSize];
        roomCenters.Clear();
        
        // Generate rooms
        int roomCount = Random.Range(roomCountMin, roomCountMax + 1);
        GenerateRoomsIsaacStyle(roomCount);
        
        // Place walls around floors
        PlaceWalls();
        
        // Instantiate tiles based on the grid
        InstantiateTiles();
    }

    private void GenerateRoomsIsaacStyle(int roomCount)
    {
        // Calculate grid cell size (room + spacing)
        int cellSize = roomSize + roomSpacing;
        
        // Start with a center room
        int centerX = dungeonSize / 2;
        int centerY = dungeonSize / 2;
        CreateRoom(centerX, centerY);
        roomCenters.Add(new Vector2Int(centerX, centerY));
        
        // List of potential room positions
        List<Vector2Int> potentialRoomPositions = new List<Vector2Int>();
        
        // Add adjacent positions to the center room
        AddPotentialRoomPositions(centerX, centerY, potentialRoomPositions);
        
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

    private void CreateRoom(int centerX, int centerY)
    {
        int halfSize = roomSize / 2;
        
        // Fill the room with floor tiles
        for (int x = centerX - halfSize; x <= centerX + halfSize; x++)
        {
            for (int y = centerY - halfSize; y <= centerY + halfSize; y++)
            {
                if (x >= 0 && x < dungeonSize && y >= 0 && y < dungeonSize)
                {
                    dungeonGrid[x, y] = 1; // floor
                }
            }
        }
    }

    private bool CanPlaceRoom(int centerX, int centerY)
    {
        int halfSize = roomSize / 2;
        
        // Check if the room would be within bounds
        if (centerX - halfSize < 0 || centerX + halfSize >= dungeonSize ||
            centerY - halfSize < 0 || centerY + halfSize >= dungeonSize)
        {
            return false;
        }
        
        // Check if the room would overlap with existing rooms
        for (int x = centerX - halfSize - 1; x <= centerX + halfSize + 1; x++)
        {
            for (int y = centerY - halfSize - 1; y <= centerY + halfSize + 1; y++)
            {
                if (x >= 0 && x < dungeonSize && y >= 0 && y < dungeonSize)
                {
                    if (dungeonGrid[x, y] == 1) // If we find a floor tile
                    {
                        return false; // Room would overlap
                    }
                }
            }
        }
        
        return true;
    }

    private void AddPotentialRoomPositions(int centerX, int centerY, List<Vector2Int> positions)
    {
        int cellSize = roomSize + roomSpacing;
        
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
            bool alreadyExists = false;
            foreach (Vector2Int pos in positions)
            {
                if (pos.x == newPos.x && pos.y == newPos.y)
                {
                    alreadyExists = true;
                    break;
                }
            }
            
            // Also check if it's already a room center
            foreach (Vector2Int pos in roomCenters)
            {
                if (pos.x == newPos.x && pos.y == newPos.y)
                {
                    alreadyExists = true;
                    break;
                }
            }
            
            if (!alreadyExists)
            {
                positions.Add(newPos);
            }
        }
    }

    private void ConnectAdjacentRooms()
    {
        int cellSize = roomSize + roomSpacing;
        int corridorWidth = 3;
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
                            if (x >= 0 && x < dungeonSize && y >= 0 && y < dungeonSize)
                            {
                                dungeonGrid[x, y] = 1;
                            }
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
                            if (x >= 0 && x < dungeonSize && y >= 0 && y < dungeonSize)
                            {
                                dungeonGrid[x, y] = 1;
                            }
                        }
                    }
                }
            }
        }
    }

    private void PlaceWalls()
{
    // Create a temporary copy of the grid to identify edge tiles
    int[,] tempGrid = new int[dungeonSize, dungeonSize];
    for (int x = 0; x < dungeonSize; x++)
    {
        for (int y = 0; y < dungeonSize; y++)
        {
            tempGrid[x, y] = dungeonGrid[x, y];
        }
    }
    
    // Check each cell in the grid
    for (int x = 0; x < dungeonSize; x++)
    {
        for (int y = 0; y < dungeonSize; y++)
        {
            // Only check floor tiles
            if (tempGrid[x, y] == 1) // Floor
            {
                // Define adjacent positions (4 directions - only cardinal directions)
                int[] dx = { 0, 1, 0, -1 };
                int[] dy = { 1, 0, -1, 0 };
                
                // Check if this floor tile is on the edge (has at least one empty neighbor)
                bool isEdge = false;
                for (int i = 0; i < 4; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];
                    
                    // Check if out of bounds or empty
                    if (nx < 0 || nx >= dungeonSize || ny < 0 || ny >= dungeonSize || tempGrid[nx, ny] == 0)
                    {
                        isEdge = true;
                        break;
                    }
                }
                
                // If this is an edge floor tile, also mark it as a wall
                // We're keeping it as floor (1) AND adding it as wall (2)
                // The InstantiateTiles method will need to check for this when placing tiles
                if (isEdge)
                {
                    // Special value 3 means "both floor and wall"
                    dungeonGrid[x, y] = 3; // Both floor and wall
                }
            }
        }
    }
}

    private void InstantiateTiles()
    {
    for (int x = 0; x < dungeonSize; x++)
    {
        for (int y = 0; y < dungeonSize; y++)
        {
            Vector3Int position = new Vector3Int(x, y, 0);
            
            if (dungeonGrid[x, y] == 1 || dungeonGrid[x, y] == 3) // floor or both
            {
                floorTilemap.SetTile(position, floorRuleTile);
            }
            
            if (dungeonGrid[x, y] == 2 || dungeonGrid[x, y] == 3) // wall or both
            {
                wallTilemap.SetTile(position, wallRuleTile);
            }
        }
    }
    }
}
