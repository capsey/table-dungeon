using UnityEngine;
using UnityEngine.InputSystem;

namespace TableDungeon.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 5.0F;
        public float acceleration = 1.0F;
    
        private Rigidbody2D _rigidbody;
        private Vector2 _targetVelocity;
        private GameManager _manager;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _manager = FindObjectOfType<GameManager>();
            _manager.Controls.Dungeon.Movement.performed += OnInputMovement;
            _manager.Controls.Dungeon.Movement.canceled += OnInputMovement;
            _manager.OnStateChanged += (s, p, changed) =>
            {
                if (!changed) return;
                transform.localPosition = Vector3.zero;
                _rigidbody.velocity = Vector2.zero;
            };
        }

        private void OnInputMovement(InputAction.CallbackContext ctx)
        {
            _targetVelocity = ctx.ReadValue<Vector2>() * speed;
        }
    
        private void Update()
        {
            var delta = acceleration * Time.deltaTime;
            _rigidbody.velocity = Vector2.MoveTowards(_rigidbody.velocity, _targetVelocity, delta);
        }
    }
}
