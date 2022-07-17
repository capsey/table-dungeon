using System;
using JetBrains.Annotations;
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
        [CanBeNull] private Chest _chest;
        private GameManager _manager;
        
        public Collider2D Player { get; set; }

        public event Action<Item> OnPlayerCollected;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _renderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            _manager = FindObjectOfType<GameManager>();
            _manager.Controls.Dungeon.Accept.performed += OnAcceptPerformed;
        }

        private void OnAcceptPerformed(InputAction.CallbackContext ctx)
        {
            if (_chest == null || _chest.looted) return;
            if (!_collider.IsTouching(Player)) return;
            
            OnPlayerCollected?.Invoke(_chest.item);
            _chest.looted = true;
            _renderer.sprite = opened;
        }

        public void SetChest([CanBeNull] Chest chest)
        {
            gameObject.SetActive(chest != null);
            _renderer.sprite = chest != null && chest.looted ? opened : closed;
            _chest = chest;
        }
    }
}