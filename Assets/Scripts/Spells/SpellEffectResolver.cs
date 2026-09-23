using Spellbound.Enemies;
using Spellbound.Scoring;
using Spellbound.Tower;

namespace Spellbound.Spells
{
    /// <summary>
    /// Applies a spell's effect to a single enemy the moment a piercing projectile hits it,
    /// or restores tower HP for Heal (which has no projectile at all).
    /// </summary>
    public static class SpellEffectResolver
    {
        public static void ApplyHeal(SpellData spell, ScoreManager scoreManager)
        {
            TowerHealth.Instance?.Heal(spell.healAmount);
            scoreManager?.RegisterSuccessfulCast(spell);
        }

        public static void ApplyToEnemy(SpellData spell, Enemy enemy)
        {
            if (enemy == null) return;

            switch (spell.effectType)
            {
                case SpellEffectType.Damage:
                    enemy.TakeDamage(spell.damage);
                    break;

                case SpellEffectType.Slow:
                    enemy.ApplySlow(spell.slowFactor, spell.slowDuration);
                    break;
            }
        }
    }
}