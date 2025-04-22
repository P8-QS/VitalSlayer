using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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

        public Tilemap wallsTilemap;

        public List<DoorInfo> doorData = new();

        [Header("Enemy Settings")]
        [Tooltip("Tag used to identify enemies that need to be defeated")]
        public string enemyTag = nameof(Fighter);

        [HideInInspector]
        public List<DoorController> connectedDoors = new();

        [HideInInspector]
        public bool isPlayerInside;

        private readonly List<Transform> _doorTransforms = new();
        public readonly List<GameObject> RoomEnemies = new();
        private Bounds? _calculatedBounds;
        private bool _isCleared;


#if UNITY_EDITOR
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
        private bool firstCheck = true;
        private float doorCloseDelay = 0.5f;
        private bool shouldCloseDoors = false;
        private float doorCloseTimer = 0f;
        private void Awake()
        {
            FindDoorTransforms();
            //Awake is ran before enemies have been spawned, this this always resulting in doors not acting as expected.
            //FindEnemiesInRoom();
        }

        private void Update()
        {
            while (RoomEnemies.Count == 0 && firstCheck)
            {
                FindEnemiesInRoom();
                firstCheck = false;
            }

            if (_isCleared)
                return;

            if (isPlayerInside && RoomEnemies.Count > 0 && !shouldCloseDoors)
            {
                shouldCloseDoors = true;
                doorCloseTimer = 0f;

            }
            if (shouldCloseDoors)
            {
                doorCloseTimer += Time.deltaTime;
                if (doorCloseTimer >= doorCloseDelay)

                {
                    foreach (var door in connectedDoors)
                    {
                        door.Close();
                    }
                }
            }

            // Iterate through enemies to see if any are still alive
            var allDefeated = true;
            for (var i = RoomEnemies.Count - 1; i >= 0; i--)
            {
                if (RoomEnemies[i])
                {
                    allDefeated = false;
                    break;
                }

                RoomEnemies.RemoveAt(i);
            }

            // If all enemies are defeated and the player is inside the room, open the doors
            if (allDefeated && isPlayerInside)
            {
                //_isCleared = true;

                foreach (var door in connectedDoors)
                {
                    door.Open();
                }
            }
        }

        public void FindEnemiesInRoom()
        {
            RoomEnemies.Clear();
            GetOrCalculateRoomBounds();

            var taggedObjects = GameObject.FindGameObjectsWithTag(enemyTag);

            foreach (var obj in taggedObjects)
            {
                if (obj.name != "Player" && _calculatedBounds != null && _calculatedBounds.Value.Contains(obj.transform.position))
                {
                    RoomEnemies.Add(obj);
                }
            }
        }

        private void FindDoorTransforms()
        {
            _doorTransforms.Clear();

            foreach (Transform child in transform)
            {
                if (!child.name.StartsWith("Door_")) continue;

                var dir = GetDoorDirection(child);
                if (dir == null) continue;

                if (doorData.Any(data => data.direction == dir && Vector3.Distance(data.localPosition, child.localPosition) < 0.01f))
                {
                    _doorTransforms.Add(child);
                }
                else
                {
                    Debug.LogWarning($"Door transform {child.name} found but no matching entry in doorData list.", this);
                }
            }
        }

        public List<Transform> GetDoorTransforms()
        {
            if (_doorTransforms.Count == 0)
            {
                FindDoorTransforms();
            }
            return _doorTransforms;
        }

        public List<DoorInfo> GetDoorPrefabData()
        {
            return doorData;
        }

        public static string GetDoorDirection(Transform door)
        {
            if (door is null || !door.name.Contains("_")) return null;
            return door.name.Split('_')[1]; // Assumes "Door_Direction" format
        }

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

        public Bounds GetOrCalculateRoomBounds()
        {
            _calculatedBounds ??= CalculateRoomBoundsAt(transform.position);
            return _calculatedBounds.Value;
        }

        public Bounds CalculateRoomBoundsAt(Vector3 simulatedWorldPosition)
        {
            float minX = 0, minY = 0, maxX = 0, maxY = 0;

            if (doorData.Count == 4)
            {
                foreach (var door in doorData)
                {
                    switch (door.direction)
                    {
                        case "North": maxY = door.localPosition.y; break;
                        case "South": minY = door.localPosition.y; break;
                        case "East":  maxX = door.localPosition.x; break;
                        case "West":  minX = door.localPosition.x; break;
                    }
                
                }
            }
            else
            {
                var floorTM = GetComponentsInChildren<Tilemap>().FirstOrDefault(t => t.gameObject.name == "Floor");
                if (floorTM)
                {
                    var minLocal = floorTM.CellToLocal(floorTM.cellBounds.min);
                    var maxLocal = floorTM.CellToLocal(floorTM.cellBounds.max);
                
                    minX = minLocal.x;
                    minY = minLocal.y;
                    maxX = maxLocal.x;
                    maxY = maxLocal.y;
                }
            }
            
            var bounds = new Bounds();
            bounds.SetMinMax(new Vector2(minX, minY), new Vector2(maxX, maxY));
            bounds.center += simulatedWorldPosition;

            return bounds;
        }
    }
}