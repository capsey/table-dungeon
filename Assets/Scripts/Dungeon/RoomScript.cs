using UnityEngine;

namespace TableDungeon.Dungeon
{
    public class RoomScript : MonoBehaviour
    {
        private Room _room = new Room();
        public DirectionMap<Door> doors;
    
        void Start()
        {
            OnRoomChanged();
            foreach (var direction in Directions.Values)
            {
                doors[direction].onPlayerEntered += OnPlayerEntered;
            }
        }

        private void OnRoomChanged()
        {
            foreach (var direction in Directions.Values)
            {
                doors[direction].gameObject.SetActive(_room.doors[direction] != null);
                doors[direction].Target = _room.doors[direction];
            }
        }

        private void OnPlayerEntered(Direction direction, Room target, Transform player)
        {
            _room = target;
            OnRoomChanged();

            var offset = (Vector3) direction.GetVector() * 2;
            player.position = doors[direction.Opposite()].transform.position + offset;
            
            Debug.Log("Room changed!");
        }
    }
}
