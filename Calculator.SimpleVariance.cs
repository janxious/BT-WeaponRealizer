using System.Text;
using BattleTech;

namespace WeaponRealizer
{
    static partial class Calculator
    {
        private static class SimpleVariance
        {
            public static bool IsNotApplicable(Weapon weapon)
            {
                return !Core.ModSettings.SimpleVariance || 
                       weapon.weaponDef.DamageVariance == 0;
            }

            public static float Calculate(Weapon weapon, float rawDamage)
            {
                var damagePerShot = weapon.DamagePerShot;
                var adjustment = rawDamage / damagePerShot;
                var variance = weapon.weaponDef.DamageVariance;
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
}