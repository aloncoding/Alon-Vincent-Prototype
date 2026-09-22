using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Spellbound.Spells;

namespace Spellbound.UI
{
    /// <summary>
    /// Shows the dot/dash sequence the player is building in real time, and flashes
    /// "MISCAST" or the spell's name when a sequence resolves.
    /// </summary>
    public class InputFeedbackUI : MonoBehaviour
    {
        public SpellCaster spellCaster;
        public TMP_Text currentSequenceText;
        public TMP_Text feedbackText;
        public float feedbackDisplayTime = 0.8f;

        void OnEnable()
        {
            if (currentSequenceText != null) currentSequenceText.text = "";
            if (feedbackText != null) feedbackText.text = "";
            spellCaster.OnSequenceChanged += UpdateSequenceText;
            spellCaster.OnSpellCast += HandleCast;
            spellCaster.OnMiscast += HandleMiscast;
        }

        void OnDisable()
        {
            if (spellCaster == null) return;
            spellCaster.OnSequenceChanged -= UpdateSequenceText;
            spellCaster.OnSpellCast -= HandleCast;
            spellCaster.OnMiscast -= HandleMiscast;
        }

        void UpdateSequenceText(IReadOnlyList<SpellSymbol> seq)
        {
            var parts = new List<string>();
            foreach (var s in seq) parts.Add(s == SpellSymbol.Tap ? "\u2022" : "\u2014");
            currentSequenceText.text = string.Join(" ", parts);
        }

        void HandleCast(SpellData spell, int lane)
        {
            StopAllCoroutines();
            StartCoroutine(ShowFeedback($"{spell.spellName.ToUpper()}!", Color.cyan));
        }

        void HandleMiscast()
        {
            StopAllCoroutines();
            StartCoroutine(ShowFeedback("MISCAST", Color.red));
        }

        IEnumerator ShowFeedback(string msg, Color color)
        {
            feedbackText.text = msg;
            feedbackText.color = color;
            yield return new WaitForSeconds(feedbackDisplayTime);
            feedbackText.text = "";
        }
    }
}