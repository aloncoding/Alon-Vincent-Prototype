using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Spellbound.Tower;
using Spellbound.Scoring;
using Spellbound.Waves;

namespace Spellbound.UI
{
    /// <summary>
    /// Drives the always-visible HUD: tower HP bar, wave number, score, and combo readout.
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

        void OnEnable()
        {
            towerHealth.OnHealthChanged += UpdateHP;
            scoreManager.OnScoreChanged += UpdateScore;
            scoreManager.OnComboChanged += UpdateCombo;
            waveManager.OnWaveStarted += UpdateWave;
        }

        void OnDisable()
        {
            if (towerHealth != null) towerHealth.OnHealthChanged -= UpdateHP;
            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged -= UpdateScore;
                scoreManager.OnComboChanged -= UpdateCombo;
            }
            if (waveManager != null) waveManager.OnWaveStarted -= UpdateWave;
        }

        void UpdateHP(float current, float max) => hpFillImage.fillAmount = current / max;
        void UpdateScore(int score) => scoreText.text = $"SCORE {score}";
        void UpdateCombo(int combo) => comboText.text = combo >= 3 ? $"COMBO x{1 + combo / 3}" : "";
        void UpdateWave(int wave) => waveText.text = $"WAVE {wave}";
    }
}