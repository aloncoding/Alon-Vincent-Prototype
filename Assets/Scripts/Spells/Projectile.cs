using UnityEngine;
using Spellbound.Core;
using Spellbound.Enemies;
using Spellbound.Lanes;
using Spellbound.Scoring;

namespace Spellbound.Spells
{
    /// <summary>
    /// The visible, physical form of a cast spell. Spawned by SpellCaster at the wizard's
    /// position on the target lane, travels right along that lane, and resolves its effect
    /// the instant it hits an enemy's trigger collider (or is discarded as a miss if it
    /// sails past every enemy in the lane).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [Tooltip("Safety net: destroyed after this long even if nothing else stops it.")]
        public float maxLifetime = 4f;

        private SpellData _spell;
        private Lane _lane;
        private ScoreManager _scoreManager;
        private bool _hasImpacted;

        public void Launch(SpellData spell, Lane lane, ScoreManager scoreManager)
        {
            _spell = spell;
            _lane = lane;
            _scoreManager = scoreManager;

            if (spell.castVfxPrefab != null)
                Instantiate(spell.castVfxPrefab, transform.position, Quaternion.identity);
            AudioManager.Instance?.PlayClip(spell.castSfx);

            Destroy(gameObject, maxLifetime);
        }

        void Update()
        {
            if (_spell == null) return;

            transform.position += Vector3.right * _spell.projectileSpeed * Time.deltaTime;

            // Sailed past every enemy in the lane without hitting anything - it's a miss.
            if (_lane != null && transform.position.x > _lane.spawnPoint.position.x + 1f)
            {
                Destroy(gameObject);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_hasImpacted || _spell == null) return;

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null) return;

            Impact(enemy);
        }

        void Impact(Enemy hitEnemy)
        {
            _hasImpacted = true;

            SpellEffectResolver.Apply(_spell, _lane, _scoreManager, hitEnemy);

            if (_spell.impactVfxPrefab != null)
                Instantiate(_spell.impactVfxPrefab, transform.position, Quaternion.identity);
            AudioManager.Instance?.PlayClip(_spell.impactSfx);

            Destroy(gameObject);
        }
    }
}