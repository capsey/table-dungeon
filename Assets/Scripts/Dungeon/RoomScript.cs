using System;
using System.Collections.Generic;
using TableDungeon.Player;
using UnityEngine;

namespace TableDungeon.Dungeon
{
    public class RoomScript : MonoBehaviour
    {
        public DirectionMap<Door> doors;
        public ChestScript[] chests;
        public Decoration[] decorations;
        [Space]
        public PlayerMovement player;

        public event Action<Direction> OnPlayerMoved;
        public event Action<Item> OnPlayerCollected;

        private void Start()
        {
            var playerCollider = player.GetComponent<Collider2D>();
            
            foreach (var direction in Directions.Values)
            {
                doors[direction].onPlayerEntered += room => OnPlayerEntered(direction, room);
            }
            
            foreach (var chest in chests)
            {
                chest.Player = playerCollider;
                chest.OnPlayerCollected += OnPlayerCollected;
            }
        }

        public void SetRoom(Room value)
        {
            var random = value.Random;
            
            foreach (var direction in Directions.Values)
            {
                doors[direction].gameObject.SetActive(value.doors[direction] != null);
                doors[direction].Target = value.doors[direction];
            }

            for (var i = 0; i < chests.Length; i++)
            {
                chests[i].SetItem(value.chests[i]);
            }
            
            foreach (var decoration in decorations)
            {
                decoration.Randomize(random, 0.25F);
            }
        }

        private void OnPlayerEntered(Direction direction, Room target)
        {
            SetRoom(target);

            var offset = (Vector3) direction.GetVector() * 2;
            player.transform.position = doors[direction.Opposite()].transform.position + offset;
            
            OnPlayerMoved?.Invoke(direction);
        }
    }
}
