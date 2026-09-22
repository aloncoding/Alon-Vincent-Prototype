using System.Collections;
using UnityEngine;
using Spellbound.Lanes;
using Spellbound.Scoring;
using Spellbound.Tower;
using Spellbound.Waves;

namespace Spellbound.Enemies
{
    /// <summary>
    /// A single spawned monster. Walks left along its lane toward the tower, can be
    /// damaged or slowed by spells, and either dies to spell damage or reaches the tower
    /// and deals damage there. Either way it reports back to WaveManager so waves know
    /// when they're clear. Needs a Collider2D (Is Trigger checked) so Projectile can
    /// detect hits, and a SpriteRenderer for the hit-flash feedback.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Enemy : MonoBehaviour
    {
        public EnemyData data;
        public int laneIndex;

        [Header("Hit Feedback")]
        [SerializeField] private Color hitFlashColor = Color.white;
        [SerializeField] private float hitFlashDuration = 0.08f;

        private float _currentHP;
        private float _currentSpeed;
        private Lane _lane;
        private float _spawnTime;
        private Coroutine _slowRoutine;
        private Coroutine _flashRoutine;
        private SpriteRenderer _spriteRenderer;

        /// Distance remaining to the tower point, used to sort "frontmost" enemies for targeting.
        public float DistanceToTower { get; private set; }

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(EnemyData enemyData, Lane lane)
        {
            data = enemyData;
            _lane = lane;
            laneIndex = lane.laneIndex;
            _currentHP = data.maxHP;
            _currentSpeed = data.moveSpeed;
            _spawnTime = Time.time;
            transform.position = lane.spawnPoint.position;
            lane.Register(this);
        }

        void Update()
        {
            if (_lane == null) return;

            Vector3 target = _lane.towerPoint.position;
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(target.x, transform.position.y, transform.position.z),
                _currentSpeed * Time.deltaTime);

            DistanceToTower = Mathf.Abs(transform.position.x - target.x);

            if (DistanceToTower < 0.1f)
                ReachTower();
        }

        public void TakeDamage(float amount)
        {
            _currentHP -= amount;

            if (_currentHP <= 0f)
            {
                Die();
                return;
            }

            FlashHit();
        }

        public void ApplySlow(float factor, float duration)
        {
            if (_slowRoutine != null) StopCoroutine(_slowRoutine);
            _slowRoutine = StartCoroutine(SlowRoutine(factor, duration));
        }

        IEnumerator SlowRoutine(float factor, float duration)
        {
            _currentSpeed = data.moveSpeed * (1f - factor);
            yield return new WaitForSeconds(duration);
            _currentSpeed = data.moveSpeed;
        }

        void FlashHit()
        {
            if (_spriteRenderer == null) return;
            if (_flashRoutine != null) StopCoroutine(_flashRoutine);
            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        IEnumerator FlashRoutine()
        {
            Color original = _spriteRenderer.color;
            _spriteRenderer.color = hitFlashColor;
            yield return new WaitForSeconds(hitFlashDuration);
            _spriteRenderer.color = original;
        }

        void Die()
        {
            ScoreManager.Instance?.RegisterKill(this, Time.time - _spawnTime);
            _lane.Unregister(this);
            WaveManager.Instance?.NotifyEnemyRemoved();
            Destroy(gameObject);
        }

        void ReachTower()
        {
            TowerHealth.Instance?.TakeDamage(data.damageToTower);
            _lane.Unregister(this);
            WaveManager.Instance?.NotifyEnemyRemoved();
            Destroy(gameObject);
        }
    }
}