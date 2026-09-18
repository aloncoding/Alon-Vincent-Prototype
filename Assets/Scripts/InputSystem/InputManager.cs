using System;
using UnityEngine;
using Spellbound.Spells;

namespace Spellbound.InputSystem
{
    /// <summary>
    /// Reads the single game input (Spacebar), classifies each press as a Tap or a Hold,
    /// and broadcasts symbols for the rest of the game (SpellCaster, UI, audio) to react to.
    /// This is the only script that touches Input directly - everything else only ever
    /// sees SpellSymbol events, which keeps the one-button constraint enforced in one place.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [Header("Timing Thresholds")]
        [Tooltip("Presses shorter than this are a Tap. Longer are a Hold.")]
        public float holdThreshold = 0.25f;
        [Tooltip("If the player pauses this long mid-sequence with no input, the current sequence resets.")]
        public float sequenceTimeout = 2.0f;

        /// Fired the instant a press is released and classified as Tap or Hold.
        public event Action<SpellSymbol> OnSymbolEntered;
        /// Fired when a pause longer than sequenceTimeout clears an in-progress sequence.
        public event Action OnSequenceCleared;
        /// Fired the moment Space goes down (for the "tink" press feedback).
        public event Action OnPressStarted;
        /// Fired once a held press crosses holdThreshold while still held (for charging VFX/SFX).
        public event Action OnHoldChargeBegin;
        /// Fired the moment Space is released, before classification.
        public event Action OnReleased;

        private float _pressStartTime;
        private bool _isPressed;
        private bool _holdChargeFired;
        private float _lastInputTime;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Update()
        {
            HandleKey();
            HandleTimeout();
        }

        void HandleKey()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isPressed = true;
                _holdChargeFired = false;
                _pressStartTime = Time.time;
                OnPressStarted?.Invoke();
            }

            if (_isPressed && !_holdChargeFired && Time.time - _pressStartTime >= holdThreshold)
            {
                _holdChargeFired = true;
                OnHoldChargeBegin?.Invoke();
            }

            if (Input.GetKeyUp(KeyCode.Space) && _isPressed)
            {
                _isPressed = false;
                float duration = Time.time - _pressStartTime;
                OnReleased?.Invoke();
                SpellSymbol symbol = duration < holdThreshold ? SpellSymbol.Tap : SpellSymbol.Hold;
                _lastInputTime = Time.time;
                OnSymbolEntered?.Invoke(symbol);
            }
        }

        void HandleTimeout()
        {
            if (!_isPressed && _lastInputTime > 0f && Time.time - _lastInputTime > sequenceTimeout)
            {
                _lastInputTime = 0f;
                OnSequenceCleared?.Invoke();
            }
        }

        /// Called by SpellCaster whenever a sequence resolves (cast or miscast) so the
        /// timeout clock restarts cleanly instead of firing immediately on the next frame.
        public void NotifyInputConsumed()
        {
            _lastInputTime = 0f;
        }
    }
}
