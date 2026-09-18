using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spellbound.Core
{
    public enum SfxId { Tap, HoldCharge, CastSuccess, Miscast, EnemyDeath, TowerHit }

    [Serializable]
    public struct SfxEntry
    {
        public SfxId id;
        public AudioClip clip;
    }

    /// <summary>
    /// Central SFX player. Wire InputManager/SpellCaster events to Play() calls
    /// (see SCENE_HIERARCHY.md) to get the "tink / whoooom / BOOM / bzzt" feedback
    /// described in the design doc.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public SfxEntry[] sfxEntries;
        public AudioSource sfxSource;

        private Dictionary<SfxId, AudioClip> _map;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _map = new Dictionary<SfxId, AudioClip>();
            foreach (var e in sfxEntries) _map[e.id] = e.clip;
        }

        public void Play(SfxId id)
        {
            if (_map != null && _map.TryGetValue(id, out var clip) && clip != null)
                sfxSource.PlayOneShot(clip);
        }
    }
}
