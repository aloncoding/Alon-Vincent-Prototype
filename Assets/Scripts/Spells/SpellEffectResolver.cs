using Spellbound.Lanes;
using Spellbound.Scoring;
using Spellbound.Tower;

namespace Spellbound.Spells
{
    /// <summary>
    /// Applies a successfully-cast spell's effect to whatever lane the targeting
    /// indicator was on the moment the cast completed.
    /// </summary>
    public static class SpellEffectResolver
    {
        public static void Apply(SpellData spell, Lane lane, ScoreManager scoreManager)
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
                    lane.GetFrontmostEnemy()?.TakeDamage(spell.damage);
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
