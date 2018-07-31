namespace WeaponRealizer
{
    public class Settings
    {
        public bool simpleVariance = true;
        public bool SimpleVariance => simpleVariance;

        public float standardDeviationPercentOfSimpleVariance = 75f;
        public float StandardDeviationSimpleVarianceMultiplier => standardDeviationPercentOfSimpleVariance / 100.0f;

        public bool overheatModifier = true;
        public bool OverheatModifier => overheatModifier;

        public bool heatDamageModifier = true;
        public bool HeatDamageModifier => heatDamageModifier;

        public bool ballisticNumberOfShots = true;
        public bool BallisticNumberOfShots => ballisticNumberOfShots;

        public bool debug = false;
    }
}