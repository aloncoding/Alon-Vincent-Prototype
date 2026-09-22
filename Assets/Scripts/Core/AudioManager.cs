using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spellbound.Core
{
    // System-level sounds not tied to a specific spell (input feedback, miscast, wave
    // clear, etc.). Per-spell cast/impact sounds are played directly via PlayClip()
    // instead, since they live on the SpellData asset rather than in this fixed list.
    public enum SfxId { Tap, HoldCharge, Miscast, EnemyDeath, TowerHit, WaveCleared }

    [Serializable]
    public struct SfxEntry
    {
        public SfxId id;
        public AudioClip clip;
    }

    /// <summary>
    /// Central SFX player. Play(id) covers the fixed system sounds in SfxId; PlayClip(clip)
    /// plays an arbitrary AudioClip on demand, used for each spell's own castSfx/impactSfx.
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

        public void PlayClip(AudioClip clip)
        {
            if (clip != null) sfxSource.PlayOneShot(clip);
        }
    }
}