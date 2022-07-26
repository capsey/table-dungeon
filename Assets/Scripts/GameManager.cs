using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TableDungeon
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Camera tableCamera;
        [SerializeField] private Camera dungeonCamera;
        [Space]
        [SerializeField] private float moveDuration = 60;
        [Space]
        [SerializeField] private Renderer figurineRenderer;
        [SerializeField] private TextMeshProUGUI currentPlayerText;
        [SerializeField] private Image timerImage;
            
        public GameState State { get; private set; }
        public bool Player1 { get; private set; }
        public Controls Controls { get; private set; }

        public Camera TableCamera => tableCamera;

        public event Action<GameState, bool, bool> OnStateChanged;

        private float _timer;

        public void EndMove() => SetGameState(GameState.Table, !Player1);
        
        private void Awake()
        {
            Controls = new Controls();
            SetGameState(GameState.Table, true);

            // Only for debug purposes! Remove ASAP!!
            Controls.Global.ToggleView.performed += _ => SetGameState((GameState) (((int) State + 1) % 2), Player1);
            Controls.Global.EndMove.performed += _ => EndMove();
        }

        private void OnDestroy() => Controls.Disable();

        private void Update()
        {
            _timer += Time.deltaTime * State switch
            {
                GameState.Table => 0.25F,
                GameState.Dungeon => 1.0F,
                _ => throw new ArgumentOutOfRangeException()
            };
            if (_timer > moveDuration) EndMove();
            timerImage.fillAmount = 1.0F - _timer / moveDuration;
        }

        public void SetGameState(GameState value, bool player1)
        {
            var changed = player1 != Player1;
            if (changed) _timer = 0;

            currentPlayerText.text = player1 ? "Player 1" : "Player 2";
            figurineRenderer.material.color = player1 ?
                Color.HSVToRGB(0.01f, 0.58f, 0.79f) : Color.HSVToRGB(0.63f, 0.42f, 0.62f);
            
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