using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class Room : MonoBehaviour
    {
        [Serializable]
        public struct DoorInfo
        {
            public string direction;
            public Vector3 localPosition;
        }
        
        public List<DoorInfo> doorData = new();
    
        private List<Transform> doorTransforms = new();
        private Bounds? calculatedBounds;

#if UNITY_EDITOR
        [ContextMenu("Find Doors and Populate Data")]
        public void PopulateDoorDataFromChildren()
        {
            doorData.Clear();
            UnityEditor.Undo.RecordObject(this, "Populate Room Door Data"); // For Undo support
            foreach (Transform child in transform)
            {
                if (!child.name.StartsWith("Door_")) continue;
                
                var dir = GetDoorDirection(child);
                if (dir is null) continue;
                
                doorData.Add(new DoorInfo
                {
                    direction = dir,
                    localPosition = child.localPosition
                });
            }
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log($"Populated door data for {gameObject.name}", this);
        }
#endif


        private void Awake()
        {
            FindDoorTransforms(); // Still useful for runtime connections
        }

        // Find actual transforms at runtime
        private void FindDoorTransforms()
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
        }

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
            return doorData;
        }

        // Helper to get the direction string from a door transform name
        public static string GetDoorDirection(Transform door)
        {
            if (door is null || !door.name.Contains("_")) return null;
            return door.name.Split('_')[1]; // Assumes "Door_Direction" format
        }

        // Helper to get the opposite direction
        public static string GetOppositeDirection(string direction)
        {
            return direction switch
            {
                "North" => "South",
                "South" => "North",
                "East" => "West",
                "West" => "East",
                _ => null
            };
        }


        // Calculate the bounds based on all child Tilemaps
        // IMPORTANT: This assumes Tilemaps are children and use the scene's main Grid
        public Bounds GetRoomBounds()
        { ;
            if (calculatedBounds.HasValue)
            {
                return calculatedBounds.Value;
            }

            var result = new Bounds();

            float minX = 0;
            float minY = 0;
            float maxX = 0;
            float maxY = 0;
        
            foreach (var door in doorData)
            {
                switch (door.direction)
                {
                    case "North": 
                        maxY =  door.localPosition.y;
                        break;
                    case "South":
                        minY =  door.localPosition.y;
                        break;
                    case "East":
                        maxX =  door.localPosition.x;
                        break;
                    case "West":
                        minX =  door.localPosition.x;
                        break;
                }
            }
        
            result.SetMinMax(new Vector2(minX, minY), new Vector2(maxX, maxY));
            
            calculatedBounds = result;
            return calculatedBounds.Value;
        }

        // Helper to invalidate cached bounds if room moves (though generator places it once)
        public void InvalidateBoundsCache() {
            calculatedBounds = null;
        }
    }
}