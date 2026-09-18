using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound.InputSystem;
using Spellbound.Lanes;
using Spellbound.Scoring;

namespace Spellbound.Spells
{
    /// <summary>
    /// The heart of the game. Listens to raw Tap/Hold symbols from InputManager, builds up
    /// the current input sequence, and resolves it against the Spellbook:
    ///   - invalid prefix (matches no spell at all)      -> MISCAST, clear buffer
    ///   - exact match, and no longer spell shares it     -> cast immediately
    ///   - exact match, but a longer spell could extend it -> wait briefly (commitDelay)
    ///     in case the player keeps typing toward the longer spell; otherwise commit
    ///   - valid but incomplete prefix                    -> keep waiting for more input
    /// Whichever lane LaneManager reports as current at the instant of casting is the
    /// lane the spell fires into - this is what ties Morse input to lane-timing strategy.
    /// </summary>
    public class SpellCaster : MonoBehaviour
    {
        public Spellbook spellbook;
        public LaneManager laneManager;
        public ScoreManager scoreManager;

        [Tooltip("How long to wait after a symbol that exactly completes a shorter spell, " +
                 "in case the player is actually typing toward a longer spell that shares the same prefix.")]
        public float commitDelay = 0.45f;

        public System.Action<IReadOnlyList<SpellSymbol>> OnSequenceChanged;
        public System.Action<SpellData, int> OnSpellCast; // spell, laneIndex it fired into
        public System.Action OnMiscast;

        private readonly List<SpellSymbol> _currentSequence = new List<SpellSymbol>();
        private Coroutine _commitRoutine;

        void Start()
        {
            InputManager.Instance.OnSymbolEntered += HandleSymbol;
            InputManager.Instance.OnSequenceCleared += HandleTimeoutClear;
        }

        void OnDisable()
        {
            if (InputManager.Instance == null) return;
            InputManager.Instance.OnSymbolEntered -= HandleSymbol;
            InputManager.Instance.OnSequenceCleared -= HandleTimeoutClear;
        }

        void HandleSymbol(SpellSymbol symbol)
        {
            _currentSequence.Add(symbol);
            OnSequenceChanged?.Invoke(_currentSequence);

            if (_commitRoutine != null) StopCoroutine(_commitRoutine);

            bool isValidPrefix = spellbook.IsPrefixOfAnySpell(_currentSequence);
            if (!isValidPrefix)
            {
                Miscast();
                return;
            }

            SpellData exactMatch = spellbook.FindExactMatch(_currentSequence);
            bool extendable = spellbook.IsExtendableToLongerSpell(_currentSequence);

            if (exactMatch != null && !extendable)
            {
                Cast(exactMatch);
            }
            else if (exactMatch != null && extendable)
            {
                _commitRoutine = StartCoroutine(CommitAfterDelay(exactMatch));
            }
            // else: valid partial sequence, not a spell yet - keep waiting for more input.
        }

        IEnumerator CommitAfterDelay(SpellData fallbackSpell)
        {
            yield return new WaitForSeconds(commitDelay);
            Cast(fallbackSpell);
        }

        void Cast(SpellData spell)
        {
            int lane = laneManager.CurrentLaneIndex;
            SpellEffectResolver.Apply(spell, laneManager.GetLane(lane), scoreManager);
            OnSpellCast?.Invoke(spell, lane);
            ClearSequence();
        }

        void Miscast()
        {
            scoreManager?.RegisterMiscast();
            OnMiscast?.Invoke();
            ClearSequence();
        }

        void HandleTimeoutClear()
        {
            if (_currentSequence.Count > 0) ClearSequence();
        }

        void ClearSequence()
        {
            if (_commitRoutine != null) { StopCoroutine(_commitRoutine); _commitRoutine = null; }
            _currentSequence.Clear();
            OnSequenceChanged?.Invoke(_currentSequence);
            InputManager.Instance.NotifyInputConsumed();
        }
    }
}
