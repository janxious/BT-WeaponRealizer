using UnityEngine;

namespace WeaponRealizer
{
    static partial class Calculator
    {
        private static class NormalDistribution
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
    }
}