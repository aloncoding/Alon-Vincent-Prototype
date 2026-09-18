using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Spellbound.Spells;

namespace Spellbound.UI
{
    /// <summary>
    /// Renders the always-visible spellbook panel (name + dot/dash pattern per unlocked
    /// spell) and dims out any spell that can no longer match the sequence the player is
    /// currently typing, so the player gets live feedback on which spells are still "live".
    /// </summary>
    public class SpellbookUI : MonoBehaviour
    {
        public Spellbook spellbook;
        public SpellCaster spellCaster;
        [Tooltip("Prefab with an Image on the root and two child TMP_Text objects named 'Name' and 'Pattern'.")]
        public GameObject rowPrefab;
        public Transform rowContainer;

        private readonly Dictionary<SpellData, GameObject> _rows = new Dictionary<SpellData, GameObject>();

        void Start()
        {
            foreach (var spell in spellbook.unlockedSpells)
            {
                GameObject row = Instantiate(rowPrefab, rowContainer);
                row.transform.Find("Name").GetComponent<TMP_Text>().text = spell.spellName.ToUpper();
                row.transform.Find("Pattern").GetComponent<TMP_Text>().text = spell.MorseDisplay;
                _rows[spell] = row;
            }
            spellCaster.OnSequenceChanged += HighlightMatches;
        }

        void OnDestroy()
        {
            if (spellCaster != null) spellCaster.OnSequenceChanged -= HighlightMatches;
        }

        void HighlightMatches(IReadOnlyList<SpellSymbol> current)
        {
            foreach (var kvp in _rows)
            {
                bool isPossible = current.Count == 0 || IsPrefix(current, kvp.Key.sequence);
                var image = kvp.Value.GetComponent<Image>();
                if (image != null)
                    image.color = isPossible ? Color.white : new Color(1f, 1f, 1f, 0.35f);
            }
        }

        bool IsPrefix(IReadOnlyList<SpellSymbol> prefix, SpellSymbol[] full)
        {
            if (prefix.Count > full.Length) return false;
            for (int i = 0; i < prefix.Count; i++)
                if (prefix[i] != full[i]) return false;
            return true;
        }
    }
}