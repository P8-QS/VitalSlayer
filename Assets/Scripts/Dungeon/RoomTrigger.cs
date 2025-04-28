using System;
using System.Collections;
using UnityEngine;

namespace Dungeon
{
    public class RoomTrigger : MonoBehaviour
    {
        public event Action OnPlayerEnterRoom;

        private Room _room;
        private Collider2D _collider;

        private void Awake()
        {
            _room = GetComponentInParent<Room>();
            _collider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            var roomGo = transform.parent.gameObject;
            var room = new RoomInstance(roomGo, _room);
            MinimapManager.Instance.UpdateMinimap(room);
            
            StartCoroutine(CheckIfPlayerIsInsideRoom(other.transform));
        }
        
        private IEnumerator CheckIfPlayerIsInsideRoom(Transform playerTransform)
        {
            if (_collider is not BoxCollider2D boxCollider) yield break;
            
            var bounds = boxCollider.bounds;
            bounds.min += new Vector3(0.05f, 0.15f, 0);
            bounds.max += new Vector3(-0.05f, -0.05f, 0);
            
            while (!bounds.Contains(playerTransform.position))
            {
                yield return null;
            }
            
            OnPlayerEnterRoom?.Invoke();
        }
    }
}