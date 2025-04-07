
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DungeonGenerator)), CanEditMultipleObjects]
public class DungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var generator = (DungeonGenerator)target;
        
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Dungeon"))
        {
            generator.GenerateDungeon();
        }
        
        if (GUILayout.Button("Clear Dungeon"))
        {
            generator.ClearDungeon();
        }
        GUILayout.EndHorizontal();
        
    }
}
