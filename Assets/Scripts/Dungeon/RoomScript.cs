using System;
using UnityEngine;

namespace TableDungeon.Dungeon
{
    public class RoomScript : MonoBehaviour
    {
        public DirectionMap<Door> doors;

        public event Action<Direction> onPlayerMoved;

        private void Start()
        {
            foreach (var direction in Directions.Values)
            {
                doors[direction].onPlayerEntered += (room, player) => OnPlayerEntered(direction, room, player);
            }
        }

        public void SetRoom(Room value)
        {
            foreach (var direction in Directions.Values)
            {
                doors[direction].gameObject.SetActive(value.doors[direction] != null);
                doors[direction].Target = value.doors[direction];
            }
        }

        private void OnPlayerEntered(Direction direction, Room target, Transform player)
        {
            SetRoom(target);

            var offset = (Vector3) direction.GetVector() * 2;
            player.position = doors[direction.Opposite()].transform.position + offset;
            
            onPlayerMoved?.Invoke(direction);
        }
    }
}
