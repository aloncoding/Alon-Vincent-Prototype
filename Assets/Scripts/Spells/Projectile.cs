using System.Collections.Generic;
using UnityEngine;
using Spellbound.Core;
using Spellbound.Enemies;
using Spellbound.Lanes;
using Spellbound.Scoring;

namespace Spellbound.Spells
{
    /// <summary>
    /// The visible, physical form of a cast spell. Spawned by SpellCaster at the wizard's
    /// position on the target lane, travels right along that lane, and applies its effect
    /// to every enemy it hits - up to spell.pierceCount of them - before being destroyed.
    /// A pierceCount of 1 behaves like the old "single target"; a high pierceCount behaves
    /// like the old "area of effect", since it just keeps going until it runs out of enemies
    /// or pierces. Each hit spawns its own impact feedback, so piercing multiple enemies
    /// reads as a satisfying chain rather than one lump-sum resolution.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [Tooltip("Safety net: destroyed after this long even if nothing else stops it.")]
        public float maxLifetime = 4f;

        private SpellData _spell;
        private Lane _lane;
        private ScoreManager _scoreManager;
        private int _remainingPierces;
        private bool _castRegistered;
        private readonly HashSet<Enemy> _hitEnemies = new HashSet<Enemy>();

        public void Launch(SpellData spell, Lane lane, ScoreManager scoreManager)
        {
            _spell = spell;
            _lane = lane;
            _scoreManager = scoreManager;
            _remainingPierces = Mathf.Max(1, spell.pierceCount);

            if (spell.castVfxPrefab != null)
                Instantiate(spell.castVfxPrefab, transform.position, Quaternion.identity);
            AudioManager.Instance?.PlayClip(spell.castSfx);

            Destroy(gameObject, maxLifetime);
        }

        void Update()
        {
            if (_spell == null) return;

            transform.position += Vector3.right * _spell.projectileSpeed * Time.deltaTime;

            // Sailed past the end of the lane - nothing left to pierce, so it's spent.
            if (_lane != null && transform.position.x > _lane.spawnPoint.position.x + 1f)
            {
                Destroy(gameObject);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_spell == null || _remainingPierces <= 0) return;

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null || _hitEnemies.Contains(enemy)) return;

            HandleHit(enemy);
        }

        void HandleHit(Enemy enemy)
        {
            _hitEnemies.Add(enemy);

            // The cast only counts toward score/combo once, on the first enemy it connects
            // with - piercing five enemies shouldn't quintuple the combo bonus.
            if (!_castRegistered)
            {
                _castRegistered = true;
                _scoreManager?.RegisterSuccessfulCast(_spell);
            }

            SpellEffectResolver.ApplyToEnemy(_spell, enemy);

            if (_spell.impactVfxPrefab != null)
                Instantiate(_spell.impactVfxPrefab, enemy.transform.position, Quaternion.identity);
            AudioManager.Instance?.PlayClip(_spell.impactSfx);

            _remainingPierces--;
            if (_remainingPierces <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}