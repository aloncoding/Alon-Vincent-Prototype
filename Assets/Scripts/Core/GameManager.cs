using System;
using System.Collections;
using UnityEngine;
using Spellbound.Waves;

namespace Spellbound.Core
{
    public enum GameState { Countdown, Playing, GameOver }

    /// <summary>
    /// Top-level game flow. No main menu: the scene starts, runs a short countdown, then
    /// waves run endlessly (scaling up forever via WaveManager's procedural generation)
    /// until the tower's HP reaches zero. GameOver is the only end state.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; } = GameState.Countdown;
        public event Action<GameState> OnStateChanged;

        [Header("Countdown")]
        [Tooltip("Seconds shown before the first wave starts, e.g. 3, 2, 1.")]
        public int countdownSeconds = 3;
        /// Fired once per second during the countdown with the number remaining (0 = "GO").
        public event Action<int> OnCountdownTick;

        public WaveManager waveManager;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            SetState(GameState.Countdown);
            StartCoroutine(CountdownThenStart());
        }

        IEnumerator CountdownThenStart()
        {
            for (int i = countdownSeconds; i > 0; i--)
            {
                OnCountdownTick?.Invoke(i);
                yield return new WaitForSeconds(1f);
            }
            OnCountdownTick?.Invoke(0); // "GO"
            yield return new WaitForSeconds(0.5f);

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