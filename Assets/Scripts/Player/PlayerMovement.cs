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

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            InputManager.Controls.Player.Movement.performed += OnInputMovement;
            InputManager.Controls.Player.Movement.canceled += OnInputMovement;
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