using UnityEngine;
using Spellbound.Core;

namespace Spellbound.UI
{
    /// <summary>
    /// Shows/hides the HUD and Game Over panels based on GameManager's current state, and
    /// forwards the restart button click back into GameManager. The game starts straight
    /// into Playing - no menu, no countdown.
    /// </summary>
    public class GameStateUI : MonoBehaviour
    {
        public GameManager gameManager;
        public GameObject hudPanel;
        public GameObject gameOverPanel;

        void OnEnable() => gameManager.OnStateChanged += HandleStateChanged;
        void OnDisable() { if (gameManager != null) gameManager.OnStateChanged -= HandleStateChanged; }

        void Start() => HandleStateChanged(gameManager.State);

        void HandleStateChanged(GameState state)
        {
            hudPanel.SetActive(state == GameState.Playing);
            gameOverPanel.SetActive(state == GameState.GameOver);
        }

        // Hook this up to the Restart Button's OnClick() in the Inspector.
        public void OnRestartButtonPressed() => gameManager.RestartGame();
    }
}