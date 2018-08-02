using System;
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