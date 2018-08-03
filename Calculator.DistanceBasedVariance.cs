using System;
using BattleTech;
using UnityEngine;

namespace WeaponRealizer
{
    internal static partial class Calculator
    {
        private static class DistanceBasedVariance
        {
            private const string DistanceVarianceTag = "WR-variance_by_distance";
            private const double Pi2 = Math.PI / 2.0;

            public static bool IsApplicable(Weapon weapon)
            {
                return Core.ModSettings.DistanceBasedVariance &&
                       weapon.weaponDef.ComponentTags.Contains(DistanceVarianceTag);
            }

            public static float Calculate(AbstractActor attacker, ICombatant target, Weapon weapon, float damage, float rawDamage)
            {
                var damagePerShot = weapon.DamagePerShot;
                var adjustment = rawDamage / damagePerShot;
                var baseMultiplier = Core.ModSettings.DistanceBasedVarianceMaxRangeDamageMultiplier;
                float varianceMultiplier;
                var distance = Vector3.Distance(attacker.TargetPosition, target.TargetPosition);
                var distanceDifference = weapon.MaxRange - distance;
                var distanceRatio = distanceDifference / weapon.MaxRange;
                var distanceBasedFunctionMultiplier = (float) Math.Atan(Pi2 * (distanceRatio) + baseMultiplier);
                if (distance < weapon.MaxRange)
                {
                    varianceMultiplier = Mathf.Max(
                        baseMultiplier,
                        Mathf.Min(
                            1.0f,
                            distanceBasedFunctionMultiplier
                        ));
                }
                else // out of range
                {
                    return damage;
                }
                Logger.Debug($"madness: {distanceBasedFunctionMultiplier}\n" +
                             $"varianceMultiplier: {varianceMultiplier}\n" +
                             $"adjustment: {adjustment}\n" +
                             $"damage: {damage}\n" +
                             $"distance: {distance}\n" +
                             $"max: {weapon.MaxRange}\n" +
                             $"distanceDifference {distanceDifference}\n" +
                             $"basemultplier: {baseMultiplier}\n" +
                             $"distanceRatio: {distanceRatio}");
                return damage * varianceMultiplier * adjustment;
            }
        }
    }
}