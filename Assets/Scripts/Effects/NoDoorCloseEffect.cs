using System;
using System.Collections.Generic;
using Dungeon;
using UnityEngine;

namespace Effects
{
    public class NoDoorCloseEffect : IEffect
    {
        public string Name => "Doors Always Open";
        public int Level { get => _level; set => _level = value; }
        
        
        private int _level;
        public Sprite Icon => _icon;
        private Sprite _icon;
        
        public NoDoorCloseEffect(Sprite icon, int level)
        {
            _icon = icon;
            _level = level;
        }
        public string Text()
        {
            return $"<color=#3AE75B>doors always open</color>";
        }

        public string Description()
        {
            return Level switch
            {
                1 => "Doors will never be closed.",
                0 => "Doors will be closed, and you need to kill all the enemies within a room for doors to open.",
                _ => "Must be level 0 or 1."
            };
        }

        public void Apply()
        {
            var doorControllers = UnityEngine.Object.FindObjectsOfType<DoorController>();
            if(doorControllers == null || doorControllers.Length == 0)
            {
                Debug.LogWarning("No DoorController found in the scene.");
                return;
            }

            if (Level == 1)
            {
                
                foreach (var door in doorControllers)
                {
                    door.Open();
                }

                
                var rooms = UnityEngine.Object.FindObjectsOfType<Room>();
                foreach (var room in rooms)
                {
                    
                    room._isCleared = true;

                    
                    for (int i = room.RoomEnemies.Count - 1; i >= 0; i--)
                    {
                        if (room.RoomEnemies[i] != null)
                        {
                            UnityEngine.Object.Destroy(room.RoomEnemies[i]);
                        }
                    }
                    room.RoomEnemies.Clear();
                }

                Debug.Log("No Door Close effect applied: All doors will remain open.");
            }
            else 
            {
                Debug.Log("Doors will close when entering rooms and open when all enemies are defeated");
            }
        }
    }
}