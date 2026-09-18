using System;
using UnityEngine;
using Spellbound.Core;

namespace Spellbound.Tower
{
    /// <summary>
    /// The wizard's tower HP. Enemies that reach it deal damage here; the Heal spell
    /// restores it. Hitting zero ends the game.
    /// </summary>
    public class TowerHealth : MonoBehaviour
    {
        public static TowerHealth Instance { get; private set; }

        public float maxHP = 100f;
        public float CurrentHP { get; private set; }

        public event Action<float, float> OnHealthChanged; // current, max

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            CurrentHP = maxHP;
        }

        public void TakeDamage(float amount)
        {
            CurrentHP = Mathf.Max(0f, CurrentHP - amount);
            OnHealthChanged?.Invoke(CurrentHP, maxHP);
            if (CurrentHP <= 0f) GameManager.Instance?.GameOver();
        }

        public void Heal(float amount)
        {
            CurrentHP = Mathf.Min(maxHP, CurrentHP + amount);
            OnHealthChanged?.Invoke(CurrentHP, maxHP);
        }
    }
}
