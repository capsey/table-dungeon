using System;
using UnityEngine;

namespace TableDungeon
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Camera tableCamera;
        [SerializeField] private Camera dungeonCamera;
        public GameState State { get; private set; }
        public Controls Controls { get; private set; }

        private void Awake()
        {
            Controls = new Controls();
            SetGameState(GameState.Table);

            // Only for debug purposes! Remove ASAP!!
            Controls.DEBUG.ToggleView.performed += _ => SetGameState((GameState) (((int) State + 1) % 2));
        }

        public void SetGameState(GameState value)
        {
            State = value;
            
            Controls.Disable();
            tableCamera.enabled = false;
            dungeonCamera.enabled = false;
            
            switch (value)
            {
                case GameState.Table:
                    tableCamera.enabled = true;
                    break;
                case GameState.Dungeon:
                    Controls.Player.Enable();
                    dungeonCamera.enabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            
            // Only for debug purposes! Remove ASAP!!
            Controls.DEBUG.Enable();
        }
    }

    public enum GameState
    {
        Table, Dungeon
    }
}