using System.Collections.Generic;
using UnityEngine;

namespace Spellbound.Spells
{
    /// <summary>
    /// Holds every spell the player currently has access to and answers the pattern-matching
    /// questions SpellCaster needs: "does this sequence exactly complete a spell?" and
    /// "could more input still turn this into a longer spell?" That second question is what
    /// lets Firebolt (•) and Fireball (• • —) coexist without one accidentally firing early.
    /// </summary>
    public class Spellbook : MonoBehaviour
    {
        public static Spellbook Instance { get; private set; }

        [Tooltip("Spells the player currently has access to.")]
        public List<SpellData> unlockedSpells = new List<SpellData>();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void UnlockSpell(SpellData spell)
        {
            if (spell != null && !unlockedSpells.Contains(spell))
                unlockedSpells.Add(spell);
        }

        public SpellData FindExactMatch(IReadOnlyList<SpellSymbol> sequence)
        {
            foreach (var spell in unlockedSpells)
                if (SequencesEqual(spell.sequence, sequence)) return spell;
            return null;
        }

        /// True if this (possibly complete) sequence is the start of at least one unlocked
        /// spell that is STRICTLY LONGER - i.e. typing more could still change the outcome.
        public bool IsExtendableToLongerSpell(IReadOnlyList<SpellSymbol> sequence)
        {
            foreach (var spell in unlockedSpells)
            {
                if (spell.sequence.Length <= sequence.Count) continue;
                if (IsPrefix(sequence, spell.sequence)) return true;
            }
            return false;
        }

        /// True if the sequence so far could still legally become some unlocked spell
        /// (exact match or a valid prefix of a longer one). False means miscast.
        public bool IsPrefixOfAnySpell(IReadOnlyList<SpellSymbol> sequence)
        {
            foreach (var spell in unlockedSpells)
            {
                if (spell.sequence.Length < sequence.Count) continue;
                if (IsPrefix(sequence, spell.sequence)) return true;
            }
            return false;
        }

        private bool IsPrefix(IReadOnlyList<SpellSymbol> prefix, IReadOnlyList<SpellSymbol> full)
        {
            for (int i = 0; i < prefix.Count; i++)
                if (prefix[i] != full[i]) return false;
            return true;
        }

        private bool SequencesEqual(IReadOnlyList<SpellSymbol> a, IReadOnlyList<SpellSymbol> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++) if (a[i] != b[i]) return false;
            return true;
        }
    }
}