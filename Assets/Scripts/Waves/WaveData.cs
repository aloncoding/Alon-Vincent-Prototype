using System;
using UnityEngine;
using Spellbound.Enemies;

namespace Spellbound.Waves
{
    [Serializable]
    public class SpawnEntry
    {
        public EnemyData enemyData;
        [Range(0, 2)] public int laneIndex;
        [Tooltip("Seconds after the wave starts that this enemy spawns.")]
        public float spawnTime;
    }

    /// <summary>
    /// A hand-authored wave: a fixed list of (enemy, lane, spawn time) entries.
    /// Create one asset per wave via Assets > Create > Spellbound > Wave.
    /// Once WaveManager runs out of hand-authored waves it falls back to
    /// procedurally generated endless waves (see WaveManager.GenerateProceduralWave).
    /// </summary>
    [CreateAssetMenu(fileName = "NewWave", menuName = "Spellbound/Wave")]
    public class WaveData : ScriptableObject
    {
        public string waveName = "Wave 1";
        public SpawnEntry[] spawns;
    }
}
