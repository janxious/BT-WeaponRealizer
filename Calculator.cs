using BattleTech;

namespace WeaponRealizer
{
    static partial class Calculator
    {
        private const float Epsilon = 0.0001f;

        static float ApplyDamageModifiers(AbstractActor attacker, ICombatant target, Weapon weapon, float rawDamage)
        {
            if (SimpleVariance.IsNotApplicable(weapon) &&
                OverheatMultiplier.IsNotApplicable(weapon) &&
                HeatDamageModifier.IsNotApplicable(weapon) &&
                HeatAsNormalDamage.IsNotApplicable(weapon, target))
            {
                return rawDamage;
            }

            var damage = rawDamage;

            if (Core.ModSettings.SimpleVariance)
            {
                damage = SimpleVariance.Calculate(weapon, rawDamage);
            }

            if (Core.ModSettings.OverheatModifier)
            {
                damage = OverheatMultiplier.Calculate(attacker, target, weapon, damage);
            }

            if (Core.ModSettings.HeatDamageModifier)
            {
                // TODO: this can't work becuse the values don't get ingested from weapondef
                // damage = HeatDamageModifier.Calculate(weapon, damage);
            }

            if (Core.ModSettings.HeatDamageAppliesToBuildingAsNormalDamage ||
                Core.ModSettings.HeatDamageAppliesToTurretAsNormalDamage ||
                Core.ModSettings.HeatDamageAppliesToVehicleAsNormalDamage)
            {
                damage = HeatAsNormalDamage.Calculate(target, weapon, damage, rawDamage);
            }

            return damage;
        }
    }
}