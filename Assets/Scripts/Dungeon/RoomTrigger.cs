using System;
using UnityEngine;

namespace Dungeon
{
    public class RoomTrigger : MonoBehaviour
    {
        public event Action OnPlayerEnterRoom;

        private Room _room;
        private bool _hasBeenTriggered;

        private void Awake()
        {
            _room = GetComponentInParent<Room>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_hasBeenTriggered) return;
            if (!other.CompareTag("Player")) return;

            _hasBeenTriggered = true;
            _room.isPlayerInside = true;

            var roomGo = transform.parent.gameObject;
            var room = new RoomInstance(roomGo, _room);
            MinimapManager.Instance.UpdateMinimap(room);

            OnPlayerEnterRoom?.Invoke();
        }
    }
}