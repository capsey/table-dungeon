using System;
using UnityEngine;

namespace TableDungeon.Dungeon
{
    public class Door : MonoBehaviour
    {
        public event Action<Room, Transform> onPlayerEntered;
        
        public Room Target { private get; set; }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            onPlayerEntered?.Invoke(Target, other.transform);
        }
    }
}
