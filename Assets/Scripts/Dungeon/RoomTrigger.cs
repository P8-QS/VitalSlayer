using System;
using UnityEngine;

namespace Dungeon
{
    public class RoomTrigger : MonoBehaviour
    {
        public event Action OnPlayerEnterRoom;

        private Room _room;

        private void Awake()
        {
            _room = GetComponentInParent<Room>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var roomGo = transform.parent.gameObject;
            var room = new RoomInstance(roomGo, _room);
            MinimapManager.Instance.UpdateMinimap(room);

            if (!other.CompareTag("Player"))
            {
                return;
            }

            _room.isPlayerInside = true;
            OnPlayerEnterRoom?.Invoke();
        }
    }
}