using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System; // For Serializable attribute

public class Room : MonoBehaviour
{
    [Serializable] // Make struct serializable so it shows in Inspector/saves
    public struct DoorInfo
    {
        public string direction; // "North", "East", etc.
        public Vector3 localPosition; // Position relative to room pivot
        // Add rotation if needed: public Quaternion localRotation;
    }

    // Store door data directly - maybe populate this via an Editor script or manually
    // Or populate it once in Awake/Start in the editor?
    public List<DoorInfo> doorData = new List<DoorInfo>();

    // Keep the runtime list of transforms for placed instances
    private List<Transform> doorTransforms = new List<Transform>();
    private BoundsInt? calculatedBounds = null;

    #if UNITY_EDITOR
    // Helper button in Inspector to find doors and populate doorData
    [ContextMenu("Find Doors and Populate Data")]
    void PopulateDoorDataFromChildren()
    {
        doorData.Clear();
        UnityEditor.Undo.RecordObject(this, "Populate Room Door Data"); // For Undo support
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Door_"))
            {
                string dir = GetDoorDirection(child);
                if (dir != null)
                {
                    doorData.Add(new DoorInfo
                    {
                        direction = dir,
                        localPosition = child.localPosition
                        // localRotation = child.localRotation // if needed
                    });
                }
            }
        }
         UnityEditor.EditorUtility.SetDirty(this); // Mark object as dirty to save changes
        Debug.Log($"Populated door data for {gameObject.name}", this);
    }
    #endif


    void Awake()
    {
        FindDoorTransforms(); // Still useful for runtime connections
    }

    // Find actual transforms at runtime
    void FindDoorTransforms()
    {
        doorTransforms.Clear();
        // Match children to the stored doorData based on position/direction
        // This is more robust if names aren't perfect
        foreach (Transform child in transform) {
             if (child.name.StartsWith("Door_")) { // Initial filter by name
                string dir = GetDoorDirection(child);
                if (dir != null) {
                     // Find matching data entry (optional but good practice)
                     bool dataFound = false;
                     foreach(var data in doorData) {
                         if(data.direction == dir && Vector3.Distance(data.localPosition, child.localPosition) < 0.01f) {
                             doorTransforms.Add(child);
                             dataFound = true;
                             break;
                         }
                     }
                     if (!dataFound) {
                         Debug.LogWarning($"Door transform {child.name} found but no matching entry in doorData list.", this);
                          // Optionally add it anyway, or rely solely on doorData?
                          // doorTransforms.Add(child);
                     }
                }
             }
        }

         // Alternative: Just find by name if doorData isn't populated/used at runtime
        // foreach (Transform child in transform) {
        //      if (child.name.StartsWith("Door_")) {
        //          doorTransforms.Add(child);
        //      }
        // }
    }

    // Use this at runtime when you have an INSTANCE
    public List<Transform> GetDoorTransforms()
    {
        // if (doorTransforms.Count == 0 && Application.isPlaying)
        if(doorTransforms.Count == 0)
        {
            FindDoorTransforms();
        }
        return doorTransforms;
    }

    // Use this in the GENERATOR to query PREFAB data
    public List<DoorInfo> GetDoorPrefabData()
    {
        // If data isn't populated, maybe try finding it on the fly (less ideal)
        // if (doorData.Count == 0) {
        //     #if UNITY_EDITOR
        //     PopulateDoorDataFromChildren(); // Try to populate in editor
        //     #endif
        // }
        return doorData;
    }

    // Helper to get the direction string from a door transform name
    public static string GetDoorDirection(Transform door)
    {
        if (door == null || !door.name.Contains("_")) return null;
        return door.name.Split('_')[1]; // Assumes "Door_Direction" format
    }

    // Helper to get the opposite direction
    public static string GetOppositeDirection(string direction)
    {
        switch (direction)
        {
            case "North": return "South";
            case "South": return "North";
            case "East": return "West";
            case "West": return "East";
            default: return null;
        }
    }


    // Calculate the bounds based on all child Tilemaps
    // IMPORTANT: This assumes Tilemaps are children and use the scene's main Grid
    public BoundsInt GetRoomBounds(Grid grid)
    {
        if (calculatedBounds.HasValue)
        {
            return calculatedBounds.Value;
        }

        BoundsInt totalBounds = new BoundsInt();
        bool firstMap = true;

        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();

        if (tilemaps.Length == 0)
        {
            Debug.LogWarning($"Room {name} has no Tilemaps to calculate bounds!", this);
            // Return a minimal bound at origin if no tilemaps found
             calculatedBounds = new BoundsInt(Vector3Int.zero, Vector3Int.one);
             return calculatedBounds.Value;
        }


        foreach (Tilemap tm in tilemaps)
        {
            if (tm.cellBounds.size.x > 0 || tm.cellBounds.size.y > 0) // Only consider maps with tiles
            {
                 // Transform bounds from local space of tilemap GO to world, then to cell coords of main grid
                Vector3Int minWorld = grid.WorldToCell(tm.transform.TransformPoint(grid.CellToLocal(tm.cellBounds.min)));
                Vector3Int maxWorld = grid.WorldToCell(tm.transform.TransformPoint(grid.CellToLocal(tm.cellBounds.max)));

                // Because WorldToCell floors, max needs adjustment to encompass the actual cells
                BoundsInt currentBounds = new BoundsInt();
                currentBounds.SetMinMax(minWorld, maxWorld);


                if (firstMap)
                {
                    totalBounds = currentBounds;
                    firstMap = false;
                }
                else
                {
                    // Encapsulate: Expand totalBounds to include currentBounds
                    totalBounds.SetMinMax(
                        Vector3Int.Min(totalBounds.min, currentBounds.min),
                        Vector3Int.Max(totalBounds.max, currentBounds.max)
                    );
                }
            }
        }

         // Add a small buffer if desired, e.g., to prevent walls touching directly
        // totalBounds.min -= Vector3Int.one;
        // totalBounds.max += Vector3Int.one;

        calculatedBounds = totalBounds;
        //Debug.Log($"Calculated Bounds for {name}: {calculatedBounds.Value}", this);
        return calculatedBounds.Value;
    }

     // Helper to invalidate cached bounds if room moves (though generator places it once)
    public void InvalidateBoundsCache() {
        calculatedBounds = null;
    }
}