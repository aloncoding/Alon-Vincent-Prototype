using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Spellbound.Core;
using Spellbound.Tower;
using Spellbound.Scoring;
using Spellbound.Waves;

namespace Spellbound.UI
{
    /// <summary>
    /// Drives the always-visible HUD: tower HP bar, wave number, score, combo readout,
    /// and a "wave cleared" moment - an SFX plus the wave text flashing a different color
    /// a few times during WaveManager's inter-wave delay, so clearing a wave reads as an
    /// unmistakable beat rather than a quiet number change.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        public TowerHealth towerHealth;
        public ScoreManager scoreManager;
        public WaveManager waveManager;

        public Image hpFillImage;
        public TMP_Text waveText;
        public TMP_Text scoreText;
        public TMP_Text comboText;

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
            if (scoreText != null) scoreText.text = "SCORE 0";
            if (comboText != null) comboText.text = "";
        }

        void OnEnable()
        {
            towerHealth.OnHealthChanged += UpdateHP;
            scoreManager.OnScoreChanged += UpdateScore;
            scoreManager.OnComboChanged += UpdateCombo;
            waveManager.OnWaveStarted += UpdateWave;
            waveManager.OnWaveCompleted += HandleWaveCompleted;
        }

        void OnDisable()
        {
            if (towerHealth != null) towerHealth.OnHealthChanged -= UpdateHP;
            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged -= UpdateScore;
                scoreManager.OnComboChanged -= UpdateCombo;
            }
            if (waveManager != null)
            {
                waveManager.OnWaveStarted -= UpdateWave;
                waveManager.OnWaveCompleted -= HandleWaveCompleted;
            }
        }

        void UpdateHP(float current, float max) => hpFillImage.fillAmount = current / max;
        void UpdateScore(int score) => scoreText.text = $"SCORE {score}";
        void UpdateCombo(int combo) => comboText.text = combo >= 3 ? $"COMBO x{1 + combo / 3}" : "";

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