using System;
using System.Text;
using BattleTech;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WeaponRealizer
{
    static class Calculator
    {
        static float Calculate(AbstractActor attacker, ICombatant target, Weapon weapon, float damage)
        {
//            Logger.Debug("calcing");
            if (WeaponHasNoSimpleVariance(weapon) && WeaponHasNoOverHeatMultiplier(weapon)) return damage;
//            Logger.Debug("really calcing");
            if (Core.ModSettings.SimpleVariance)
            {
                damage = DoSimpleVarianceCalculate(weapon, damage);
            }

            if (Core.ModSettings.OverheatModifier)
            {
//                Logger.Debug("checking overheat");
                Logger.Debug($"damage before overheat check: {damage}");
                damage = DoOverheatMultiplierForOverheated(attacker, target as AbstractActor, weapon, damage);
                Logger.Debug($"damage after overheat check: {damage}");
            }

            return damage;
        }

        private static bool WeaponHasNoSimpleVariance(Weapon weapon)
        {
            return !Core.ModSettings.SimpleVariance || 
                   weapon.weaponDef.DamageVariance == 0;
        }

        private const float Epsilon = 0.0001f;
        private static bool WeaponHasNoOverHeatMultiplier(Weapon weapon)
        {
//            Logger.Debug($"WeaponHasNoOverHeatMultiplier:\n" +
//                         $"enabled: {Core.ModSettings.OverheatModifier}\n" +
//                         $"value: {weapon.weaponDef.OverheatedDamageMultiplier}\n" +
//                         $"zeroish? {Mathf.Abs(weapon.weaponDef.OverheatedDamageMultiplier) < Epsilon}");
            return !Core.ModSettings.OverheatModifier ||
                   Mathf.Abs(weapon.weaponDef.OverheatedDamageMultiplier) < Epsilon;
        }

        private static float DoOverheatMultiplierForOverheated(AbstractActor attacker, AbstractActor target, Weapon weapon, float rawDamage)
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

        private static float DoSimpleVarianceCalculate(Weapon weapon, float damage, bool log = true)
        {
            var damagePerShot = weapon.DamagePerShot;
            var variance = weapon.weaponDef.DamageVariance;
            var adjustment = damage / damagePerShot;
            var roll = NormalDistibutionRandom(new VarianceBounds(damagePerShot - variance, damagePerShot + variance, Core.ModSettings.StandardDeviationSimpleVarianceMultiplier));
            var variantDamage = roll * adjustment;
            if (log)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"roll: {roll}");
                sb.AppendLine($"damagePerShot: {damagePerShot}");
                sb.AppendLine($"variance: {variance}");
                sb.AppendLine($"adjustment: {adjustment}");
                sb.AppendLine($"variantDamage: {variantDamage}");
                Logger.Debug(sb.ToString());
            }

            return variantDamage;
        }

        private const int IterationLimit = 10;
        private static float NormalDistibutionRandom(VarianceBounds bounds, int step = -1)
        {
            // compute a random number that fits a gaussian function https://en.wikipedia.org/wiki/Gaussian_function
            // iterative w/ limit adapted from https://natedenlinger.com/php-random-number-generator-with-normal-distribution-bell-curve/
            var iterations = 0;
            float randomNumber;
            do
            {
                var rand1 = Random.value;
                var rand2 = Random.value;
                var gaussianNumber = Mathf.Sqrt(-2 * Mathf.Log(rand1)) * Mathf.Cos(2 * Mathf.PI * rand2);
                var mean = (bounds.max + bounds.min) / 2;
                randomNumber = (gaussianNumber * bounds.standardDeviation) + mean;
                if (step > 0) randomNumber = Mathf.RoundToInt(randomNumber / step) * step;
                iterations++;
            } while ((randomNumber < bounds.min || randomNumber > bounds.max) && iterations < IterationLimit);

            if (iterations == IterationLimit) randomNumber = (bounds.min + bounds.max) / 2.0f;
            return randomNumber;
        }

        private static bool _doTestSimpleVarianceCalculated = false;
        private static void DoTestSimpleVarianceCalculations(Weapon weapon, float damage)
        {
            if (!_doTestSimpleVarianceCalculated) return;

            var sb = new StringBuilder();
            for (var i = 0; i < 1000; i++)
            {
                sb.AppendLine(DoSimpleVarianceCalculate(weapon, damage, false).ToString());
            }
            _doTestSimpleVarianceCalculated = false;
            Logger.Debug(sb.ToString());
        }
    }
}