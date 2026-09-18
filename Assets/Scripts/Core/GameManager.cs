using System;
using UnityEngine;
using Spellbound.Waves;

namespace Spellbound.Core
{
    public enum GameState { MainMenu, Playing, Paused, CheckpointChoice, GameOver, Victory }

    /// <summary>
    /// Top-level game flow: starts the wave loop, listens for game-over/victory
    /// conditions, and handles the "every 5 waves, cash out or continue" checkpoint.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; } = GameState.MainMenu;
        public event Action<GameState> OnStateChanged;

        public WaveManager waveManager;
        [Tooltip("If greater than 0, the game ends in Victory after this wave instead of running endlessly.")]
        public int finalWaveNumber = 0;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            waveManager.OnWaveCompleted += HandleWaveCompleted;
            waveManager.OnCheckpointReached += HandleCheckpoint;
        }

        public void StartGame()
        {
            SetState(GameState.Playing);
            waveManager.StartNextWave();
        }

        void HandleWaveCompleted(int waveNumber)
        {
            if (finalWaveNumber > 0 && waveNumber >= finalWaveNumber)
                Victory();
        }

        void HandleCheckpoint()
        {
            SetState(GameState.CheckpointChoice);
            // UI calls ContinuePastCheckpoint() or CashOut() based on the player's choice.
        }

        public void ContinuePastCheckpoint()
        {
            SetState(GameState.Playing);
            waveManager.StartNextWave();
        }

        public void CashOut() => Victory();

        public void GameOver() => SetState(GameState.GameOver);

        public void Victory() => SetState(GameState.Victory);

        public void RestartGame()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene.buildIndex);
        }

        void SetState(GameState newState)
        {
            State = newState;
            OnStateChanged?.Invoke(State);
        }
    }
}
