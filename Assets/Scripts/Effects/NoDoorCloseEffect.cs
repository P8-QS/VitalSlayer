using System;
using System.Collections.Generic;
using Dungeon;
using UnityEngine;

namespace Effects
{
    public class NoDoorCloseEffect : IEffect
    {
        public string Name => "No Door Close";
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
            return $"<color=#EA2E1E>fog level {Level}</color>";
        }

        public string Description()
        {
            return Level switch
            {
                0 => "Doors will not be closed.",
                1 => "Doors will be closed, and you need to kill all the enemies within a room for doors to open.",
                _ => "Must be level 0 or 1."
            };
        }

        public void Apply()
        {
            var doorControllers = UnityEngine.Object.FindObjectsOfType<DoorController>();
            if (doorControllers == null || doorControllers.Length == 0)
            {
                Debug.LogWarning("No door controllers found.");
                return;
            }

            if (Level == 0)
            {
                foreach (var door in doorControllers)
                {
                    door.Open();
                }


                var rooms = UnityEngine.Object.FindObjectsOfType<Room>();
                foreach (var room in rooms)
                {
                    room._isCleared = true;
                }

                Debug.Log("Noor doors closed effect applied. All doors are open.");
            } 
            else
            {
                var rooms = UnityEngine.Object.FindObjectsOfType<Room>();
                foreach (var room in rooms)
                {
                    room._isCleared = false;
                }
                Debug.Log("Door will close when entering rooms and open when all enemies are defeated.");
            }


        }
    }
}