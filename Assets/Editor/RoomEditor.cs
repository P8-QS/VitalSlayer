using Dungeon;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var room = (Room)target;
        
        GUILayout.Space(10);
        if (GUILayout.Button("Find Doors and Populate Data"))
        {
            room.PopulateDoorDataFromChildren();
        }
    }
}