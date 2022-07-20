using System;
using System.Collections;
using System.Collections.Generic;
using TableDungeon.Player;
using UnityEngine;

namespace TableDungeon.Dungeon
{
    public class RoomScript : MonoBehaviour
    {
        public DirectionMap<Door> doors;
        public DirectionMap<GameObject> rocks;
        public GameObject bomb;
        public ChestScript[] chests;
        public Decoration[] decorations;
        [Space]
        public PlayerMovement player;

        private Room _room;
        private GameManager _manager;

        public event Action<Direction> OnPlayerMoved;
        public event Action<Item> OnPlayerCollected;

        private void Start()
        {
            _manager = FindObjectOfType<GameManager>();
            var playerCollider = player.GetComponent<Collider2D>();

            foreach (var direction in Directions.Values)
            {
                doors[direction].onPlayerEntered += () => OnPlayerEntered(direction);
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
            var manager = FindObjectOfType<GameManager>();
            var bombHere = value.trap?.item == Item.Bomb && value.trap?.player1 != manager.Player1;
            
            StopAllCoroutines();
            if (bombHere) StartCoroutine(BombCoroutine());
            bomb.SetActive(bombHere);
            
            foreach (var direction in Directions.Values)
            {
                rocks[direction].SetActive(value.doors[direction] != null && value.doors[direction].blocked);
                doors[direction].gameObject.SetActive(value.doors[direction] != null && !value.doors[direction].blocked);
                doors[direction].Target = value.doors[direction];
            }

            for (var i = 0; i < chests.Length; i++)
            {
                chests[i].SetChest(value.chests[i]);
            }
            
            foreach (var decoration in decorations)
            {
                decoration.Randomize(random, 0.25F);
            }

            _room = value;
        }

        private IEnumerator BombCoroutine()
        {
            yield return new WaitForSeconds(1.5F);
            FindObjectOfType<GameManager>().EndMove();
        }

        private void OnPlayerEntered(Direction direction)
        {
            var offset = (Vector3) direction.GetVector() * 2;
            player.transform.position = doors[direction.Opposite()].transform.position + offset;

            if (_room.trap?.item == Item.Bomb && _room.trap?.player1 != _manager.Player1)
            {
                _room.trap = null;
                _room.blocked = true;
            }
            
            OnPlayerMoved?.Invoke(direction);
        }
    }
}
