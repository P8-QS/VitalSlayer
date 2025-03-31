using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : MonoBehaviour
{
    public Tilemap doorTilemap;
    public TileBase openTile;
    public TileBase closedTile;
    
    // Optional offset using float values for more precise positioning
    public Vector3 positionOffset = Vector3.zero;
    
    [HideInInspector]
    public Vector3Int doorPosition;
    
    // Detection radius for the player
    public float detectionRadius = 2f;
    
    // The player layer (set this in the Inspector)
    public LayerMask playerLayer;

    // Direct reference to the entrance collider (assign in inspector if hierarchy is different)
    public BoxCollider2D entranceCollider;
    
    private bool isOpen = false;
    private bool initialized = false;

    // Debug visualization
    public bool showDebugInfo = true;

    void Start()
    {
        if (!doorTilemap)
        {
            Debug.LogError("Door Tilemap reference is missing!");
            return;
        }

        // Find the door position and setup immediately
        InitializeDoor();
    }

    void InitializeDoor()
    {
        // Calculate the door position with floating point offset
        Vector3 adjustedPosition = transform.position + positionOffset;
        doorPosition = doorTilemap.WorldToCell(adjustedPosition);
        
        if (showDebugInfo)
        {
            Debug.Log($"Door initialized at position {doorPosition} (world: {transform.position}, adjusted: {adjustedPosition})");
        }

        // Try to find entrance collider if not manually assigned
        if (entranceCollider == null)
        {
            // Look in children first
            entranceCollider = GetComponentInChildren<BoxCollider2D>();
            
            // If still not found, look for a specific named object
            if (entranceCollider == null)
            {
                GameObject entranceObj = GameObject.Find("Entrance");
                if (entranceObj != null)
                {
                    entranceCollider = entranceObj.GetComponent<BoxCollider2D>();
                }
            }
            
            if (entranceCollider == null)
            {
                Debug.LogWarning("No entrance collider found! Door will not have collision.");
            }
        }

        // Ensure the door is initially in closed state
        isOpen = false;
        if (entranceCollider != null)
        {
            entranceCollider.enabled = true;
        }
        
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;
        
        // Check if player is nearby
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        
        // If player is detected and door is closed, open it
        if (playerCollider != null && !isOpen)
        {
            OpenDoor();
        }
        // If player walks away, close the door
        else if (playerCollider == null && isOpen)
        {
            // Uncomment this if you want doors to auto-close when player walks away
            // CloseDoor();
        }
    }
    
    public void OpenDoor()
    {
        if (doorTilemap == null) return;
        
        isOpen = true;
        
        // First, completely clear the tile at this position
        doorTilemap.SetTile(doorPosition, null);
        
        // Then set the new open door tile
        doorTilemap.SetTile(doorPosition, openTile);
        
        if (showDebugInfo)
        {
            Debug.Log($"Door opened at position {doorPosition}");
        }
        
        // Disable collider to make it passable
        if (entranceCollider != null)
        {
            entranceCollider.enabled = false;
        }
    }
    
    public void CloseDoor()
    {
        if (doorTilemap == null) return;
        
        isOpen = false;
        
        // First, completely clear the tile
        doorTilemap.SetTile(doorPosition, null);
        
        // Then set the closed door tile
        doorTilemap.SetTile(doorPosition, closedTile);
        
        if (showDebugInfo)
        {
            Debug.Log($"Door closed at position {doorPosition}");
        }
        
        // Enable collider to block passage
        if (entranceCollider != null)
        {
            entranceCollider.enabled = true;
        }
    }

    // Toggle function for other uses
    public void ToggleDoor()
    {
        if (isOpen)
            CloseDoor();
        else
            OpenDoor();
    }
    
    // Draw the detection radius in the editor for easier setup
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Calculate and show the door position in edit mode too
        if (doorTilemap != null)
        {
            // Show both the object position and the adjusted tile position
            Vector3 adjustedPosition = transform.position + positionOffset;
            Vector3Int doorPos = doorTilemap.WorldToCell(adjustedPosition);
            Vector3 doorWorldPos = doorTilemap.CellToWorld(doorPos) + new Vector3(0.5f, 0.5f, 0);
            
            // Show the actual door object position
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
            
            // Show the adjusted position
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(adjustedPosition, 0.1f);
            
            // Show the final tile position
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(doorWorldPos, Vector3.one * 0.8f);
        }
    }
}