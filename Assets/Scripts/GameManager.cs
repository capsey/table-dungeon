using System;
using UnityEngine;

namespace TableDungeon
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Camera tableCamera;
        [SerializeField] private Camera dungeonCamera;
        [Space]
        [SerializeField] private float moveDuration = 60;
        public GameState State { get; private set; }
        public bool Player1 { get; private set; }
        public Controls Controls { get; private set; }

        public Camera TableCamera => tableCamera;

        public event Action<GameState, bool, bool> OnStateChanged;

        private float _timer;

        private void Awake()
        {
            Controls = new Controls();
            SetGameState(GameState.Table, true);

            // Only for debug purposes! Remove ASAP!!
            Controls.Global.ToggleView.performed += _ => SetGameState((GameState) (((int) State + 1) % 2), Player1);
            Controls.Global.EndMove.performed += _ =>
            {
                SetGameState(GameState.Table, !Player1);
                Debug.Log("Move ended!");
            };
        }

        private void Update()
        {
            if (State == GameState.Dungeon)
            {
                _timer += Time.deltaTime;
                if (_timer > moveDuration)
                {
                    SetGameState(GameState.Table, !Player1);
                    Debug.Log("Move ended!");
                }
            }
        }

        public void SetGameState(GameState value, bool player1)
        {
            var changed = player1 != Player1;
            if (changed) _timer = 0;
            
            State = value;
            Player1 = player1;

            Controls.Disable();
            Controls.Global.Enable();
            
            tableCamera.enabled = false;
            dungeonCamera.enabled = false;
            
            switch (value)
            {
                case GameState.Table:
                    Controls.Table.Enable();
                    tableCamera.enabled = true;
                    break;
                case GameState.Dungeon:
                    Controls.Dungeon.Enable();
                    dungeonCamera.enabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            OnStateChanged?.Invoke(value, player1, changed);
        }
    }

    public enum GameState
    {
        Table, Dungeon
    }
}