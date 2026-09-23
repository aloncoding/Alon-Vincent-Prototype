using System.Collections;
using TMPro;
using UnityEngine;
using Spellbound.Core;
using Spellbound.Tower;
using Spellbound.Scoring;
using Spellbound.Waves;

namespace Spellbound.UI
{
    /// <summary>
    /// Drives the always-visible HUD: tower HP (as plain text, e.g. "HP: 80/100"), wave
    /// number, and score. A "wave cleared" moment - an SFX plus the wave text flashing a
    /// different color a few times during WaveManager's inter-wave delay - makes clearing
    /// a wave read as an unmistakable beat rather than a quiet number change.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        public TowerHealth towerHealth;
        public ScoreManager scoreManager;
        public WaveManager waveManager;

        public TMP_Text hpText;
        public TMP_Text waveText;
        public TMP_Text scoreText;

        [Header("Wave Cleared Feedback")]
        public Color waveClearedColor = Color.yellow;
        public float waveClearFlashDuration = 1.2f;
        [Tooltip("How many times the wave text blinks between its normal color and waveClearedColor.")]
        public int waveClearFlashCount = 3;

        private Color _waveTextDefaultColor;
        private Coroutine _waveClearFlashRoutine;

        void Awake()
        {
            if (waveText != null) _waveTextDefaultColor = waveText.color;
        }

        void OnEnable()
        {
            towerHealth.OnHealthChanged += UpdateHP;
            scoreManager.OnScoreChanged += UpdateScore;
            waveManager.OnWaveStarted += UpdateWave;
            waveManager.OnWaveCompleted += HandleWaveCompleted;
        }

        void OnDisable()
        {
            if (towerHealth != null) towerHealth.OnHealthChanged -= UpdateHP;
            if (scoreManager != null) scoreManager.OnScoreChanged -= UpdateScore;
            if (waveManager != null)
            {
                waveManager.OnWaveStarted -= UpdateWave;
                waveManager.OnWaveCompleted -= HandleWaveCompleted;
            }
        }

        void UpdateHP(float current, float max) =>
            hpText.text = $"HP: {Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";

        void UpdateScore(int score) => scoreText.text = $"SCORE {score}";

        void UpdateWave(int wave)
        {
            // A new wave is starting - cut off any leftover flash from the previous clear
            // and make sure the text is back to its normal color before showing the number.
            if (_waveClearFlashRoutine != null)
            {
                StopCoroutine(_waveClearFlashRoutine);
                _waveClearFlashRoutine = null;
            }
            waveText.color = _waveTextDefaultColor;
            waveText.text = $"WAVE {wave}";
        }

        void HandleWaveCompleted(int waveNumber)
        {
            AudioManager.Instance?.Play(SfxId.WaveCleared);

            if (_waveClearFlashRoutine != null) StopCoroutine(_waveClearFlashRoutine);
            _waveClearFlashRoutine = StartCoroutine(FlashWaveClearedText());
        }

        IEnumerator FlashWaveClearedText()
        {
            float segment = waveClearFlashDuration / (waveClearFlashCount * 2f);
            for (int i = 0; i < waveClearFlashCount; i++)
            {
                waveText.color = waveClearedColor;
                yield return new WaitForSeconds(segment);
                waveText.color = _waveTextDefaultColor;
                yield return new WaitForSeconds(segment);
            }
            _waveClearFlashRoutine = null;
        }
    }
}