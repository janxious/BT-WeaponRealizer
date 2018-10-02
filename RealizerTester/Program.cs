using System;
using UnityEngine;

namespace RealizerTester
{
    internal class Program
    {
        private const double Pi2 = Math.PI / 2.0;
        public static void Main(string[] args)
        {
            var damage = 50f;
            var damagePerShot = 50f;
            var adjustment = damage/damagePerShot;
            float varianceMultiplier = 0f;
            var maxRange = 120f;
            var floor = 10f;
            Console.WriteLine($"Computing for weapon with {damage} damage and a max range of {maxRange} and a floor of {floor}%");
            Console.WriteLine();
            Console.WriteLine($"Distance to target | ComputedDamage");
            Console.WriteLine($"-----------------------------------");
            for (var distance = 0f; distance <= maxRange; distance += 1f)
            {
                var distanceDifference = maxRange - distance;
                var distanceRatio = distanceDifference / maxRange;
                var baseMultiplier = floor / 100f; // the tag
                var distanceBasedFunctionMultiplier = (float) Math.Atan(Pi2 * distanceRatio + baseMultiplier);
                if (distance <= maxRange)
                {
                    varianceMultiplier = Mathf.Max(
                        baseMultiplier,
                        Mathf.Min(
                            1.0f,
                            distanceBasedFunctionMultiplier
                        ));
                }
                var computedDamage = damage * varianceMultiplier * adjustment;
                Console.WriteLine($"{distance}|{computedDamage}");
            }
        }
    }
}