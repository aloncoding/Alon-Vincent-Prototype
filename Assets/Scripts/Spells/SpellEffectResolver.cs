using Spellbound.Enemies;
using Spellbound.Lanes;
using Spellbound.Scoring;
using Spellbound.Tower;

namespace Spellbound.Spells
{
    /// <summary>
    /// Applies a spell's effect once its projectile has actually landed (or, for Heal,
    /// the instant it's cast - Heal has no projectile). "impactedEnemy" is the specific
    /// enemy a SingleTarget projectile physically collided with, if any; other effect
    /// types ignore it and act on the whole lane instead.
    /// </summary>
    public static class SpellEffectResolver
    {
        public static void Apply(SpellData spell, Lane lane, ScoreManager scoreManager, Enemy impactedEnemy = null)
        {
            if (spell.effectType == SpellEffectType.Heal)
            {
                TowerHealth.Instance?.Heal(spell.healAmount);
                scoreManager?.RegisterSuccessfulCast(spell);
                return;
            }

            if (lane == null) return;

            switch (spell.effectType)
            {
                case SpellEffectType.SingleTarget:
                    (impactedEnemy ?? lane.GetFrontmostEnemy())?.TakeDamage(spell.damage);
                    break;

                case SpellEffectType.MultiTarget:
                    foreach (var e in lane.GetEnemiesOrderedByProgress(spell.maxTargets))
                        e.TakeDamage(spell.damage);
                    break;

                case SpellEffectType.AreaOfEffect:
                    foreach (var e in lane.GetAllEnemies())
                        e.TakeDamage(spell.damage);
                    break;

                case SpellEffectType.Slow:
                    foreach (var e in lane.GetAllEnemies())
                        e.ApplySlow(spell.slowFactor, spell.slowDuration);
                    break;
            }

            scoreManager?.RegisterSuccessfulCast(spell);
        }
    }
}