#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(Room))]
public class RoomDoorEditor : Editor
{
    private bool isPlacingDoors = false;
    private bool isRemovingDoors = false;
    private Room.DoorOrientation currentDoorOrientation = Room.DoorOrientation.Front;

    public override void OnInspectorGUI()
    {
        Room room = (Room)target;

        // Draw the default inspector
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Door Tools", EditorStyles.boldLabel);

        if (room.doorTilemap == null)
        {
            EditorGUILayout.HelpBox("Please assign a Tilemap for doors first!", MessageType.Warning);
        }
        else
        {
            if (isPlacingDoors)
            {
                EditorGUILayout.LabelField("Door Placement Mode", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("Click on the tilemap to place doors. Press 'Finish' when done.", MessageType.Info);

                // Option to select door orientation
                currentDoorOrientation = (Room.DoorOrientation)EditorGUILayout.EnumPopup("Door Orientation", currentDoorOrientation);

                if (GUILayout.Button("Finish Placing Doors"))
                {
                    isPlacingDoors = false;
                    SceneView.duringSceneGui -= OnSceneGUI;
                }
            }
            else if (isRemovingDoors)
            {
                EditorGUILayout.LabelField("Door Removal Mode", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("Click on existing doors to remove them. Press 'Finish' when done.", MessageType.Info);

                if (GUILayout.Button("Finish Removing Doors"))
                {
                    isRemovingDoors = false;
                    SceneView.duringSceneGui -= OnSceneGUI;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Place Doors"))
                {
                    // Exit any other modes
                    ExitAllModes();

                    isPlacingDoors = true;
                    SceneView.duringSceneGui += OnSceneGUI;
                    Debug.Log("Click on the tilemap to place doors.");
                }

                if (GUILayout.Button("Remove Doors"))
                {
                    // Exit any other modes
                    ExitAllModes();

                    isRemovingDoors = true;
                    SceneView.duringSceneGui += OnSceneGUI;
                    Debug.Log("Click on existing doors to remove them.");
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Room room = (Room)target;
        Event currentEvent = Event.current;

        // Handle door placement/removal mode
        if ((isPlacingDoors || isRemovingDoors) && room.doorTilemap != null)
        {
            HandleDoorEditingMode(currentEvent, room);
        }
    }

    private void HandleDoorEditingMode(Event currentEvent, Room room)
    {
        // Change cursor
        EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height),
            isPlacingDoors ? MouseCursor.Arrow : MouseCursor.ArrowMinus);

        // Draw existing doors
        foreach (Room.DoorData door in room.doors)
        {
            Vector3 worldPos = room.doorTilemap.CellToWorld(door.position) +
                               (Vector3)room.doorTilemap.cellSize * 0.5f; // Center of the cell
            worldPos.z = 0;

            // Use different colors for different door orientations
            Handles.color = door.orientation == Room.DoorOrientation.Front ? Color.yellow : Color.green;
            Handles.DrawWireDisc(worldPos, Vector3.forward, 0.3f);

            // Draw a small line to indicate orientation
            Vector3 direction = door.orientation == Room.DoorOrientation.Front ? Vector3.up : Vector3.right;
            Handles.DrawLine(worldPos, worldPos + direction * 0.4f);
        }

        // Handle mouse clicks for placing/removing doors
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            Vector3 mouseWorldPos = GetMouseWorldPosition2D(currentEvent.mousePosition);

            // Convert to tilemap cell position
            Vector3Int cellPos = room.doorTilemap.WorldToCell(mouseWorldPos);

            Undo.RecordObject(room, isPlacingDoors ? "Add Door" : "Remove Door");

            if (isPlacingDoors)
            {
                room.AddDoor(cellPos, currentDoorOrientation);
                Debug.Log($"Added {currentDoorOrientation} door at cell position {cellPos}");
            }
            else if (isRemovingDoors)
            {
                // Check if there's a door at this position
                bool doorRemoved = false;
                for (int i = 0; i < room.doors.Count; i++)
                {
                    if (room.doors[i].position == cellPos)
                    {
                        room.RemoveDoor(cellPos);
                        doorRemoved = true;
                        Debug.Log($"Removed door at cell position {cellPos}");
                        break;
                    }
                }

                if (!doorRemoved)
                {
                    Debug.Log($"No door found at cell position {cellPos}");
                }
            }

            EditorUtility.SetDirty(room);
            currentEvent.Use();
        }
    }

    private Vector3 GetMouseWorldPosition2D(Vector2 mousePosition)
    {
        // Convert mouse position to world space for 2D
        Vector3 mousePos = HandleUtility.GUIPointToWorldRay(mousePosition).origin;
        mousePos.z = 0;
        return mousePos;
    }

    private void ExitAllModes()
    {
        // Exit any active modes
        isPlacingDoors = false;
        isRemovingDoors = false;

        // Unregister scene GUI callback to avoid multiple registrations
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnDisable()
    {
        // Make sure to unregister the callback when the editor is disabled
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}
#endif