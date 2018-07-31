using System;
using System.Text;
using BattleTech;
using UnityEngine;

namespace WeaponRealizer
{
    static class Calculator
    {
        static float ApplyDamageModifiers(AbstractActor attacker, ICombatant target, Weapon weapon, float damage)
        {
            if (SimpleVariance.IsNotApplicable(weapon) &&
                OverheatMultiplier.IsNotApplicable(weapon) &&
                HeatDamageModifier.IsNotApplicable(weapon))
            {
                return damage;
            }

            if (Core.ModSettings.SimpleVariance)
            {
                damage = SimpleVariance.Calculate(weapon, damage);
            }

            if (Core.ModSettings.OverheatModifier)
            {
                damage = OverheatMultiplier.Calculate(attacker, target as AbstractActor, weapon, damage);
            }

            return damage;
        }

        static class SimpleVariance
        {
            public static bool IsNotApplicable(Weapon weapon)
            {
                return !Core.ModSettings.SimpleVariance || 
                       weapon.weaponDef.DamageVariance == 0;
            }

            public static float Calculate(Weapon weapon, float damage)
            {
                var damagePerShot = weapon.DamagePerShot;
                var variance = weapon.weaponDef.DamageVariance;
                var adjustment = damage / damagePerShot;
                var roll = NormalDistribution.Random(new VarianceBounds(damagePerShot - variance, damagePerShot + variance, Core.ModSettings.StandardDeviationSimpleVarianceMultiplier));
                var variantDamage = roll * adjustment;
            
                var sb = new StringBuilder();
                sb.AppendLine($"roll: {roll}");
                sb.AppendLine($"damagePerShot: {damagePerShot}");
                sb.AppendLine($"variance: {variance}");
                sb.AppendLine($"adjustment: {adjustment}");
                sb.AppendLine($"variantDamage: {variantDamage}");
                Logger.Debug(sb.ToString());

                return variantDamage;
            }
        }
 
        static class OverheatMultiplier
        {
            private const float Epsilon = 0.0001f;
            public static bool IsNotApplicable(Weapon weapon)
            {
                return !Core.ModSettings.OverheatModifier ||
                       Mathf.Abs(weapon.weaponDef.OverheatedDamageMultiplier) < Epsilon;
            }

            public static float Calculate(AbstractActor attacker, AbstractActor target, Weapon weapon, float rawDamage)
            {
                var rawMultiplier = weapon.weaponDef.OverheatedDamageMultiplier;
                var effectActor = rawMultiplier < 0 ? attacker : target;
                var multiplier = Mathf.Abs(rawMultiplier);
                var damage = rawDamage;
                if (effectActor is Mech mech && mech.IsOverheated)
                {
                    damage = rawDamage * multiplier;
                }
                var sb = new StringBuilder();
                sb.AppendLine($"rawMultiplier: {rawMultiplier}");
                sb.AppendLine(String.Format("effectActor: {0}", rawMultiplier < 0 ? "attacker" : "target"));
                sb.AppendLine($"multiplier: {multiplier}");
                sb.AppendLine($"rawDamage: {rawDamage}");
                sb.AppendLine($"damage: {damage}");
                Logger.Debug(sb.ToString());
            
                return damage;
            }
        }

        static class HeatDamageModifier
        {
            public static bool IsNotApplicable(Weapon weapon)
            {
                // TODO: need a mechanism for this to support multiplier on both sides of an attack
                return true;
            }
        }

        static class NormalDistribution
        {
            private const int IterationLimit = 10;
            public static float Random(VarianceBounds bounds, int step = -1)
            {
                // compute a random number that fits a gaussian function https://en.wikipedia.org/wiki/Gaussian_function
                // iterative w/ limit adapted from https://natedenlinger.com/php-random-number-generator-with-normal-distribution-bell-curve/
                var iterations = 0;
                float randomNumber;
                do
                {
                    var rand1 = UnityEngine.Random.value;
                    var rand2 = UnityEngine.Random.value;
                    var gaussianNumber = Mathf.Sqrt(-2 * Mathf.Log(rand1)) * Mathf.Cos(2 * Mathf.PI * rand2);
                    var mean = (bounds.max + bounds.min) / 2;
                    randomNumber = (gaussianNumber * bounds.standardDeviation) + mean;
                    if (step > 0) randomNumber = Mathf.RoundToInt(randomNumber / step) * step;
                    iterations++;
                } while ((randomNumber < bounds.min || randomNumber > bounds.max) && iterations < IterationLimit);

                if (iterations == IterationLimit) randomNumber = (bounds.min + bounds.max) / 2.0f;
                return randomNumber;
            }
        }

//        private static bool _doTestSimpleVarianceCalculated = false;
//        private static void DoTestSimpleVarianceCalculations(Weapon weapon, float damage)
//        {
//            if (!_doTestSimpleVarianceCalculated) return;
//
//            var sb = new StringBuilder();
//            for (var i = 0; i < 1000; i++)
//            {
//                sb.AppendLine(DoSimpleVarianceCalculate(weapon, damage).ToString());
//            }
//            _doTestSimpleVarianceCalculated = false;
//            Logger.Debug(sb.ToString());
//        }
    }
}