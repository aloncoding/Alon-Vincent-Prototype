using System;
using UnityEngine;
using Spellbound.Waves;

namespace Spellbound.Core
{
    public enum GameState { Playing, GameOver }

    /// <summary>
    /// Top-level game flow. No main menu, no countdown - the scene starts and waves begin
    /// immediately. Waves run endlessly (scaling up forever via WaveManager's procedural
    /// generation) until the tower's HP reaches zero. GameOver is the only end state.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; } = GameState.Playing;
        public event Action<GameState> OnStateChanged;

        public WaveManager waveManager;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            SetState(GameState.Playing);
            waveManager.StartNextWave();
        }

        public void GameOver() => SetState(GameState.GameOver);

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