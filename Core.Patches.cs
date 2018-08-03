using Harmony;

namespace WeaponRealizer
{
    public static partial class Core
    {
        public static class Patches
        {
            public static void Apply(HarmonyInstance harmony)
            {
                if (ModSettings.BallisticNumberOfShots)
                {
                    Logger.Debug("Patching in Ballistic Number of Shots");
                    harmony.Patch(
                        original: AccessTools.Method(typeof(BallisticEffect), "Update"),
                        prefix: new HarmonyMethod(typeof(NumberOfShotsEnabler), "BallisticEffectUpdatePrefix"),
                        postfix: null
                    );
                    harmony.Patch(
                        original: AccessTools.Method(typeof(BallisticEffect), "OnComplete"),
                        prefix: new HarmonyMethod(typeof(NumberOfShotsEnabler), "BallisticEffectOnCompletePrefix"),
                        postfix: new HarmonyMethod(typeof(NumberOfShotsEnabler), "BallisticEffectOnCompletePostfix")
                    );
                }
            }
        }
    }
}