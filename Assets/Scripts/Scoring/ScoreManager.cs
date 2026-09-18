using System;
using UnityEngine;
using Spellbound.Enemies;
using Spellbound.Spells;

namespace Spellbound.Scoring
{
    /// <summary>
    /// Tracks score, combo multiplier, and cast/miscast counts (used on the victory
    /// screen: "SPELLS CAST", "MISCASTS"). A miscast resets the combo to zero.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public int Score { get; private set; }
        public int Combo { get; private set; }
        public int SpellsCast { get; private set; }
        public int Miscasts { get; private set; }

        [Tooltip("Enemy must die within this many seconds of spawning to count as a 'fast kill'.")]
        public float fastKillWindow = 2.5f;
        public int perfectCastBonus = 50;
        public int fastKillBonus = 50;

        public event Action<int> OnScoreChanged;
        public event Action<int> OnComboChanged;
        public event Action OnMiscastOccurred;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void RegisterKill(Enemy enemy, float timeAlive)
        {
            int points = enemy.data.scoreValue * ComboMultiplier();
            if (timeAlive <= fastKillWindow) points += fastKillBonus;
            AddScore(points);
        }

        public void RegisterSuccessfulCast(SpellData spell)
        {
            SpellsCast++;
            Combo++;
            OnComboChanged?.Invoke(Combo);
            AddScore(perfectCastBonus * ComboMultiplier());
        }

        public void RegisterMiscast()
        {
            Miscasts++;
            Combo = 0;
            OnComboChanged?.Invoke(Combo);
            OnMiscastOccurred?.Invoke();
        }

        int ComboMultiplier() => 1 + Combo / 3; // x1 baseline, x2 at combo 3, x3 at combo 6...

        void AddScore(int amount)
        {
            Score += amount;
            OnScoreChanged?.Invoke(Score);
        }
    }
}
