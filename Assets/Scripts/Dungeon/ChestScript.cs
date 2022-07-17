using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

namespace TableDungeon.Dungeon
{
    public class ChestScript : MonoBehaviour
    {
        [SerializeField] private Vector2 minDeviation;
        [SerializeField] private Vector2 maxDeviation;
        [Space]
        [SerializeField] private Sprite closed;
        [SerializeField] private Sprite opened;

        private Vector3 _initialPosition;
        private Collider2D _collider;
        private SpriteRenderer _renderer;
        private Item _item;
        
        public Collider2D Player { get; set; }

        public event Action<Item> onPlayerCollected;

        private void Awake()
        {
            _initialPosition = transform.position;
            _collider = GetComponent<Collider2D>();
            _renderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            var manager = FindObjectOfType<GameManager>();
            manager.Controls.Dungeon.Accept.performed += OnAcceptPerformed;
        }

        private void OnAcceptPerformed(InputAction.CallbackContext ctx)
        {
            if (!_collider.IsTouching(Player)) return;
            
            onPlayerCollected?.Invoke(_item);
            _item.looted = true;
            _renderer.sprite = opened;

            Debug.Log($"Player got an item! {_item.type}");
        }

        public void SetItem(Item item)
        {
            gameObject.SetActive(item != null);
            _renderer.sprite = item != null && item.looted ? opened : closed;
            _item = item;
        }

        public void RandomizePosition(Random random)
        {
            var min = _initialPosition + (Vector3) minDeviation;
            var max = _initialPosition + (Vector3) maxDeviation;
            transform.position = new Vector3(
                (float) random.NextRange(min.x, max.x),
                (float) random.NextRange(min.y, max.y),
                (float) random.NextRange(min.z, max.z)
            );
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) return;

            var size = maxDeviation - minDeviation;
            var center = transform.position + (Vector3) (maxDeviation + minDeviation) / 2;
            
            Gizmos.DrawWireCube(center, size);
        }
    }
}