using System;
using UnityEngine;

namespace TableDungeon.Dungeon
{
    public class Door : MonoBehaviour
    {
        public Direction direction;
        
        public event Action<Direction, Room, Transform> onPlayerEntered;
        
        public Room Target { private get; set; }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            onPlayerEntered?.Invoke(direction, Target, other.transform);
        }
    }
}
