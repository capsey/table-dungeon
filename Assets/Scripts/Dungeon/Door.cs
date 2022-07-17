using System;
using UnityEngine;

namespace TableDungeon.Dungeon
{
    public class Door : MonoBehaviour
    {
        public event Action<Room> onPlayerEntered;

        private GameManager _manager;
        
        public Room Target { private get; set; }

        private void Awake()
        {
            _manager = FindObjectOfType<GameManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (Target.state1 == Room.State.Unreachable && _manager.Player1) return;
            if (Target.state2 == Room.State.Unreachable && !_manager.Player1) return;
            onPlayerEntered?.Invoke(Target);
        }
    }
}
