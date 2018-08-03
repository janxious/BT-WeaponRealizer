namespace WeaponRealizer
{
    public class Settings
    {
        public bool simpleVariance = true;
        public bool SimpleVariance => simpleVariance;

        public float standardDeviationPercentOfSimpleVariance = 75f;
        public float StandardDeviationSimpleVarianceMultiplier => standardDeviationPercentOfSimpleVariance / 100f;

        public bool overheatModifier = true;
        public bool OverheatModifier => overheatModifier;

        public bool heatDamageModifier = true;
        public bool HeatDamageModifier => heatDamageModifier;

        public bool heatDamageAppliesToVehicleAsNormalDamage = true;
        public bool HeatDamageAppliesToVehicleAsNormalDamage => heatDamageAppliesToVehicleAsNormalDamage;

        public float heatDamagePercentApplicationToVehicle = 50f;
        public float HeatDamageApplicationToVehicleMultiplier => heatDamagePercentApplicationToVehicle / 100f;

        public bool heatDamageAppliesToTurretAsNormalDamage = true;
        public bool HeatDamageAppliesToTurretAsNormalDamage => heatDamageAppliesToTurretAsNormalDamage;

        public float heatDamagePercentApplicationToTurret = 75f;
        public float HeatDamageApplicationToTurretMultiplier => heatDamagePercentApplicationToTurret / 100f;

        public bool heatDamageAppliesToBuildingAsNormalDamage = true;
        public bool HeatDamageAppliesToBuildingAsNormalDamage => heatDamageAppliesToBuildingAsNormalDamage;

        public float heatDamagePercentApplicationToBuilding = 150f;
        public float HeatDamageApplicationToBuildingMultiplier => heatDamagePercentApplicationToBuilding / 100f;

        public bool ballisticNumberOfShots = true;
        public bool BallisticNumberOfShots => ballisticNumberOfShots;

        public bool laserNumberOfShots = true;
        public bool LaserNumberOfShots => laserNumberOfShots;

        public bool debug = false;
    }
}