using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TableDungeon.Dungeon
{
    public class ChestScript : MonoBehaviour
    {
        [Space]
        [SerializeField] private Sprite closed;
        [SerializeField] private Sprite opened;
        
        private Collider2D _collider;
        private SpriteRenderer _renderer;
        private Item _item;
        
        public Collider2D Player { get; set; }

        public event Action<Item> OnPlayerCollected;

        private void Awake()
        {
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
            
            OnPlayerCollected?.Invoke(_item);
            _item.looted = true;
            _renderer.sprite = opened;
        }

        public void SetItem(Item item)
        {
            gameObject.SetActive(item != null);
            _renderer.sprite = item != null && item.looted ? opened : closed;
            _item = item;
        }
    }
}