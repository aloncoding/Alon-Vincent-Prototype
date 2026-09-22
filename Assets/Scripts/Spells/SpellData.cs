using UnityEngine;

namespace Spellbound.Spells
{
    public enum SpellEffectType
    {
        SingleTarget,   // e.g. Fireball - high damage to the frontmost enemy in the lane
        MultiTarget,    // e.g. Lightning - hits several enemies in the lane
        AreaOfEffect,   // e.g. Meteor - hits every enemy in the lane
        Slow,           // e.g. Freeze - slows every enemy in the lane
        Heal            // e.g. Heal - restores tower HP, no lane targeting/projectile needed
    }

    /// <summary>
    /// Data-only definition of a spell: its Morse-style input sequence, its projectile,
    /// and what it does on impact. Create one asset per spell via
    /// Assets > Create > Spellbound > Spell.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSpell", menuName = "Spellbound/Spell")]
    public class SpellData : ScriptableObject
    {
        public string spellName = "New Spell";

        [Tooltip("The Tap/Hold sequence that casts this spell, in order.")]
        public SpellSymbol[] sequence;

        public SpellEffectType effectType = SpellEffectType.SingleTarget;

        [Header("Combat")]
        public float damage = 1f;
        [Tooltip("Used by MultiTarget: how many enemies in the lane are hit, closest first.")]
        public int maxTargets = 3;
        [Tooltip("Used by Slow: fraction of speed removed. 0.5 = 50% slower.")]
        [Range(0f, 1f)] public float slowFactor = 0.5f;
        public float slowDuration = 3f;
        [Tooltip("Used by Heal: tower HP restored.")]
        public float healAmount = 20f;

        [Header("Cast Feedback (played the instant the spell fires from the wizard)")]
        public AudioClip castSfx;
        public GameObject castVfxPrefab;

        [Header("Projectile & Impact (ignored by Heal, which has no projectile)")]
        [Tooltip("Prefab with a Projectile component, Collider2D (Is Trigger) and Rigidbody2D (Kinematic).")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 10f;
        [Tooltip("Spawned at the point of impact - a hit flash, burst, shatter, etc.")]
        public GameObject impactVfxPrefab;
        public AudioClip impactSfx;

        [Header("Presentation")]
        public Sprite icon;

        /// Human readable dot/dash string for the spellbook UI, e.g. "• • —".
        public string MorseDisplay
        {
            get
            {
                if (sequence == null || sequence.Length == 0) return "";
                var parts = new string[sequence.Length];
                for (int i = 0; i < sequence.Length; i++)
                    parts[i] = sequence[i] == SpellSymbol.Tap ? "\u2022" : "\u2014";
                return string.Join(" ", parts);
            }
        }
    }
}