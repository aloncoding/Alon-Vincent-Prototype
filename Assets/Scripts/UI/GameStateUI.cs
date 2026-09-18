using UnityEngine;
using Spellbound.Core;

namespace Spellbound.UI
{
    /// <summary>
    /// Shows/hides the Main Menu, HUD, Checkpoint, Game Over and Victory panels based on
    /// GameManager's current state, and forwards button clicks back into GameManager.
    /// </summary>
    public class GameStateUI : MonoBehaviour
    {
        public GameManager gameManager;
        public GameObject mainMenuPanel;
        public GameObject hudPanel;
        public GameObject checkpointPanel;
        public GameObject gameOverPanel;
        public GameObject victoryPanel;

        void OnEnable() => gameManager.OnStateChanged += HandleStateChanged;
        void OnDisable() { if (gameManager != null) gameManager.OnStateChanged -= HandleStateChanged; }

        void Start() => HandleStateChanged(gameManager.State);

        void HandleStateChanged(GameState state)
        {
            mainMenuPanel.SetActive(state == GameState.MainMenu);
            hudPanel.SetActive(state == GameState.Playing);
            checkpointPanel.SetActive(state == GameState.CheckpointChoice);
            gameOverPanel.SetActive(state == GameState.GameOver);
            victoryPanel.SetActive(state == GameState.Victory);
        }

        // Hook these up to the corresponding UI Buttons' OnClick() in the Inspector.
        public void OnPlayButtonPressed() => gameManager.StartGame();
        public void OnRestartButtonPressed() => gameManager.RestartGame();
        public void OnContinueButtonPressed() => gameManager.ContinuePastCheckpoint();
        public void OnCashOutButtonPressed() => gameManager.CashOut();
    }
}
