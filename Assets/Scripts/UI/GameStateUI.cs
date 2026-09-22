using UnityEngine;
using Spellbound.Core;

namespace Spellbound.UI
{
    /// <summary>
    /// Shows/hides the Countdown, HUD, and Game Over panels based on GameManager's current
    /// state, and forwards the restart button click back into GameManager. No main menu -
    /// the game starts straight into the countdown.
    /// </summary>
    public class GameStateUI : MonoBehaviour
    {
        public GameManager gameManager;
        public GameObject countdownPanel;
        public GameObject hudPanel;
        public GameObject gameOverPanel;

        void OnEnable() => gameManager.OnStateChanged += HandleStateChanged;
        void OnDisable() { if (gameManager != null) gameManager.OnStateChanged -= HandleStateChanged; }

        void Start() => HandleStateChanged(gameManager.State);

        void HandleStateChanged(GameState state)
        {
            countdownPanel.SetActive(state == GameState.Countdown);
            hudPanel.SetActive(state == GameState.Playing);
            gameOverPanel.SetActive(state == GameState.GameOver);
        }

        // Hook this up to the Restart Button's OnClick() in the Inspector.
        public void OnRestartButtonPressed() => gameManager.RestartGame();
    }
}