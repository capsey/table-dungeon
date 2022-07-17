using System;
using UnityEngine;

namespace TableDungeon.Dungeon
{
    public class Door : MonoBehaviour
    {
        public event Action<Room> onPlayerEntered;
        
        public Room Target { private get; set; }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Target.state == Room.State.Unreachable || !other.CompareTag("Player")) return;
            onPlayerEntered?.Invoke(Target);
        }
    }
}
