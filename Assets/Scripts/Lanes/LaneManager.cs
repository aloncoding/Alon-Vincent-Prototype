using System;
using UnityEngine;

namespace Spellbound.Lanes
{
    /// <summary>
    /// Continuously rotates the targeting indicator between the three lanes on a fixed
    /// interval. Whatever lane is "current" the instant a spell finishes casting is the
    /// lane the spell fires into - this is the crux of the game's "read -> decide -> cast"
    /// timing puzzle.
    /// </summary>
    public class LaneManager : MonoBehaviour
    {
        public static LaneManager Instance { get; private set; }

        [Tooltip("The three Lane components, top to bottom.")]
        public Lane[] lanes = new Lane[3];

        [Tooltip("Seconds the targeting indicator dwells on each lane before rotating.")]
        public float rotationInterval = 3f;

        public int CurrentLaneIndex { get; private set; } = 0;
        public event Action<int> OnLaneChanged;

        private float _timer;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            OnLaneChanged?.Invoke(CurrentLaneIndex);
        }

        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= rotationInterval)
            {
                _timer -= rotationInterval;
                CurrentLaneIndex = (CurrentLaneIndex + 1) % lanes.Length;
                OnLaneChanged?.Invoke(CurrentLaneIndex);
            }
        }

        /// 0..1 progress toward the next rotation - handy for a UI countdown ring.
        public float RotationProgress => _timer / rotationInterval;

        public Lane GetLane(int index) => lanes[index];
    }
}
