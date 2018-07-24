using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using BattleTech;
using Harmony;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WeaponRealizer
{
    internal struct VarianceBounds
    {
        public readonly float min;
        public readonly float max;
        public readonly float standardDeviation;

        public VarianceBounds(float min, float max, float standardDeviation)
        {
            this.min = min;
            this.max = max;
            this.standardDeviation = standardDeviation;
        }
    }

    static class Calculator
    {
        static bool _varianceCalculated = false;
        static float Calculate(Weapon weapon, float damage)
        {
            if (weapon.weaponDef.DamageVariance == 0)
            {
                return damage;
            }

            DoTestCalculations(weapon, damage);
            var returnValue = DoCalculate(weapon, damage);
            return returnValue;
        }

        private static void DoTestCalculations(Weapon weapon, float damage)
        {
            if (!_varianceCalculated)
            {
                StringBuilder sb = new StringBuilder();
                for (var i = 0; i < 1000; i++)
                {
                    sb.AppendLine(DoCalculate(weapon, damage, false).ToString());
                }
                _varianceCalculated = true;
                Logger.Debug(sb.ToString());
            }
        }

        private static float NormalDistibutionRandom(VarianceBounds bounds, int step = -1)
        {
            // compute a random number that fits a gaussian function https://en.wikipedia.org/wiki/Gaussian_function
            // iterative w/ limit adapted from https://natedenlinger.com/php-random-number-generator-with-normal-distribution-bell-curve/
            const int iterationLimit = 10;
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
            } while ((randomNumber < bounds.min || randomNumber > bounds.max) && iterations < iterationLimit);

            if (iterations == iterationLimit) randomNumber = (bounds.min + bounds.max) / 2.0f;
            return randomNumber;
        }

        private static float DoCalculate(Weapon weapon, float damage, bool log = true)
        {
            var damagePerShot = weapon.DamagePerShot;
            var variance = weapon.weaponDef.DamageVariance;
            var adjustment = damage / damagePerShot;
            var roll = NormalDistibutionRandom(new VarianceBounds(damagePerShot - variance, damagePerShot + variance, 0.75f));
            var variantDamage = roll * adjustment;
            var sb = new StringBuilder();
            if (log)
            {
                sb.AppendLine($"roll: {roll}");
                sb.AppendLine($"damagePerShot: {damagePerShot}");
                sb.AppendLine($"variance: {variance}");
                sb.AppendLine($"adjustment: {adjustment}");
                sb.AppendLine($"variantDamage: {variantDamage}");
                Logger.Debug(sb.ToString());
            }

            return variantDamage;
        }
    }

    /// <summary>
    /// So we have some code like this:
    ///   float hitDamage = impactMessage.hitDamage;
    ///   float qualityMultiplier = this.Director.Combat.ToHit.GetBlowQualityMultiplier(impactMessage.hitInfo.hitQualities[hitIndex]);
    ///   float num = hitDamage * qualityMultiplier;
    /// We want something like this happening at the end
    ///   num = rand(variance/weapondamage) * num
    /// </summary>
    [HarmonyPatch(typeof(AttackDirector.AttackSequence), "OnAttackSequenceImpact")]
    static class AttackSequencePatcher
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionList = instructions.ToList();
            var instructionsToInsert = new List<CodeInstruction>();                                
            var targetField = AccessTools.Field(typeof(AttackDirector.AttackSequence), "target");
            var targetFieldIndex = instructionList.FindIndex(instruction =>
                instruction.opcode == OpCodes.Ldfld && instruction.operand == targetField 
            );
            var insertionIndex = targetFieldIndex - 1;
            var calculatorMethod = AccessTools.Method(typeof(Calculator), "Calculate",
                new Type[] {typeof(Weapon), typeof(float)});

            instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, 4)); // weapon
            instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, 8)); // load "num"
            instructionsToInsert.Add(new CodeInstruction(OpCodes.Callvirt, calculatorMethod)); // call out to our calc and put result on stack
            instructionsToInsert.Add(new CodeInstruction(OpCodes.Stloc_S, 8)); // store as "num"
            instructionList.InsertRange(insertionIndex, instructionsToInsert);
                                                                                  
            return instructionList;
        }
    }
}