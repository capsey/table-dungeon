using UnityEngine;
using UnityEngine.InputSystem;

namespace TableDungeon.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 5.0F;
        public float acceleration = 1.0F;

        private SpriteRenderer _renderer;
        private Animator _animator;
        
        private Rigidbody2D _rigidbody;
        private Vector2 _targetVelocity;
        private GameManager _manager;
        
        private static readonly int MoveEnded = Animator.StringToHash("Move Ended");
        private static readonly int Player = Animator.StringToHash("Player");
        private static readonly int Moving = Animator.StringToHash("Moving");

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _manager = FindObjectOfType<GameManager>();
            _manager.Controls.Dungeon.Movement.performed += OnInputMovement;
            _manager.Controls.Dungeon.Movement.canceled += OnInputMovement;
            _manager.OnStateChanged += (s, player, changed) =>
            {
                if (!changed) return;
                transform.localPosition = Vector3.zero;
                _rigidbody.velocity = Vector2.zero;
                _animator.SetBool(Player, player);
                _animator.SetTrigger(MoveEnded);
            };
        }

        private void OnInputMovement(InputAction.CallbackContext ctx)
        {
            _targetVelocity = ctx.ReadValue<Vector2>() * speed;
            if (_targetVelocity.x > 0) _renderer.flipX = false;
            else if (_targetVelocity.x < 0) _renderer.flipX = true;
            _animator.SetBool(Moving, _targetVelocity.sqrMagnitude > 0);
        }
    
        private void Update()
        {
            var delta = acceleration * Time.deltaTime;
            _rigidbody.velocity = Vector2.MoveTowards(_rigidbody.velocity, _targetVelocity, delta);
        }
    }
}
