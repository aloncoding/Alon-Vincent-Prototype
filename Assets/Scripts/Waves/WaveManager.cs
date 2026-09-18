using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spellbound.Enemies;
using Spellbound.Lanes;

namespace Spellbound.Waves
{
    /// <summary>
    /// Drives the wave loop: spawns enemies from a WaveData asset over time, waits until
    /// every enemy in the wave is dead or has reached the tower, then either starts the
    /// next wave automatically or - every 5 waves - raises a checkpoint so the player can
    /// choose to cash out with a score multiplier or keep going (see GDD section 11).
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }

        [Tooltip("Hand-authored waves in order. When exhausted, waves are generated procedurally.")]
        public WaveData[] handAuthoredWaves;
        public LaneManager laneManager;

        [Header("Procedural pool (used once hand-authored waves run out)")]
        public EnemyData smallEnemy;
        public EnemyData mediumEnemy;
        public EnemyData largeEnemy;

        public int CurrentWaveNumber { get; private set; } = 0;
        public System.Action<int> OnWaveStarted;
        public System.Action<int> OnWaveCompleted;
        /// Raised every 5th wave instead of auto-starting the next one.
        public System.Action OnCheckpointReached;

        private int _aliveEnemiesInWave;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void StartNextWave()
        {
            CurrentWaveNumber++;
            OnWaveStarted?.Invoke(CurrentWaveNumber);
            WaveData wave = GetWaveData(CurrentWaveNumber);
            StartCoroutine(RunWave(wave));
        }

        WaveData GetWaveData(int waveNumber)
        {
            int idx = waveNumber - 1;
            if (handAuthoredWaves != null && idx < handAuthoredWaves.Length && idx >= 0)
                return handAuthoredWaves[idx];
            return GenerateProceduralWave(waveNumber);
        }

        IEnumerator RunWave(WaveData wave)
        {
            _aliveEnemiesInWave = wave.spawns.Length;

            var remaining = new List<SpawnEntry>(wave.spawns);
            remaining.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));

            float elapsed = 0f;
            foreach (var entry in remaining)
            {
                float wait = entry.spawnTime - elapsed;
                if (wait > 0f) yield return new WaitForSeconds(wait);
                elapsed = entry.spawnTime;
                SpawnEnemy(entry);
            }

            // Wave has no enemies at all - treat as instantly cleared.
            if (wave.spawns.Length == 0) yield return null;

            yield return new WaitUntil(() => _aliveEnemiesInWave <= 0);

            OnWaveCompleted?.Invoke(CurrentWaveNumber);

            if (CurrentWaveNumber % 5 == 0)
                OnCheckpointReached?.Invoke();
            else
                StartNextWave();
        }

        void SpawnEnemy(SpawnEntry entry)
        {
            Lane lane = laneManager.GetLane(entry.laneIndex);
            GameObject go = Instantiate(entry.enemyData.prefab);
            Enemy enemy = go.GetComponent<Enemy>();
            enemy.Initialize(entry.enemyData, lane);
        }

        /// Called by Enemy when it dies or reaches the tower, so the wave knows when it's clear.
        public void NotifyEnemyRemoved()
        {
            _aliveEnemiesInWave--;
        }

        WaveData GenerateProceduralWave(int waveNumber)
        {
            var wave = ScriptableObject.CreateInstance<WaveData>();
            wave.waveName = $"Wave {waveNumber}";

            int enemyCount = Mathf.Min(4 + waveNumber, 24);
            var spawns = new List<SpawnEntry>();
            float spacing = Mathf.Max(0.4f, 1.2f - waveNumber * 0.02f);

            for (int i = 0; i < enemyCount; i++)
            {
                spawns.Add(new SpawnEntry
                {
                    laneIndex = Random.Range(0, 3),
                    spawnTime = i * spacing,
                    enemyData = PickEnemyForWave(waveNumber)
                });
            }
            wave.spawns = spawns.ToArray();
            return wave;
        }

        EnemyData PickEnemyForWave(int waveNumber)
        {
            float roll = Random.value;
            if (waveNumber >= 6 && roll < 0.15f) return largeEnemy;
            if (waveNumber >= 3 && roll < 0.5f) return mediumEnemy;
            return smallEnemy;
        }
    }
}
