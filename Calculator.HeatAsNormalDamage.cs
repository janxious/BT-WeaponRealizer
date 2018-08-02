using BattleTech;

namespace WeaponRealizer
{
    static partial class Calculator
    {
        private static class HeatAsNormalDamage
        {
            public static bool IsNotApplicable(Weapon weapon, ICombatant target)
            {
                return !(Core.ModSettings.HeatDamageAppliesToBuildingAsNormalDamage ||
                         Core.ModSettings.HeatDamageAppliesToVehicleAsNormalDamage ||
                         Core.ModSettings.HeatDamageAppliesToTurretAsNormalDamage) ||
                       weapon.HeatDamagePerShot < Epsilon;

            }

            public static float Calculate(ICombatant target, Weapon weapon, float currentDamage, float rawDamage)
            {
                var damage = currentDamage;
                if (Core.ModSettings.HeatDamageAppliesToBuildingAsNormalDamage && target is Building building)
                {
                    var damagePerShot = weapon.DamagePerShot;
                    var adjustment = rawDamage / damagePerShot * Core.ModSettings.HeatDamageApplicationToBuildingMultiplier;
                    damage = currentDamage + (adjustment * weapon.HeatDamagePerShot);
                }
                else if (Core.ModSettings.HeatDamageAppliesToVehicleAsNormalDamage && target is Vehicle vehicle)
                {
                    var damagePerShot = weapon.DamagePerShot;
                    var adjustment = rawDamage / damagePerShot * Core.ModSettings.HeatDamageApplicationToVehicleMultiplier;
                    damage = currentDamage + (adjustment * weapon.HeatDamagePerShot);
                }
                else if (Core.ModSettings.HeatDamageAppliesToTurretAsNormalDamage && target is Turret turret)
                {
                    var damagePerShot = weapon.DamagePerShot;
                    var adjustment = rawDamage / damagePerShot * Core.ModSettings.HeatDamageApplicationToTurretMultiplier;
                    damage = currentDamage + (adjustment * weapon.HeatDamagePerShot);
                }
                return damage;
            }
        }
    }
}