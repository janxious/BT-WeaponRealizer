using BattleTech;

namespace WeaponRealizer
{
    static partial class Calculator
    {
        private const float Epsilon = 0.0001f;

        internal static float ApplyDamageModifiers(AbstractActor attacker, ICombatant target, Weapon weapon, float rawDamage)
        {
            var damage = rawDamage;

            if (SimpleVariance.IsApplicable(weapon))
            {
                damage = SimpleVariance.Calculate(weapon, rawDamage);
            }

            if (DistanceBasedVariance.IsApplicable(weapon))
            {
                damage = DistanceBasedVariance.Calculate(attacker, target, weapon, damage, rawDamage);
            }

            if (OverheatMultiplier.IsApplicable(weapon))
            {
                damage = OverheatMultiplier.Calculate(attacker, target, weapon, damage);
            }

            if (HeatDamageModifier.IsApplicable(weapon))
            {
                // TODO: this can't work becuse the values don't get ingested from weapondef
                // damage = HeatDamageModifier.Calculate(weapon, damage);
            }

            if (HeatAsNormalDamage.IsApplicable(weapon))
            {
                damage = HeatAsNormalDamage.Calculate(target, weapon, damage, rawDamage);
            }

            return damage;
        }
    }
}