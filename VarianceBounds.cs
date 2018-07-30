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
}