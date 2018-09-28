using System;
using System.Reflection.Emit;
using Harmony;

namespace WeaponRealizer
{
    [HarmonyPatch(typeof(BallisticEffect), "OnComplete", MethodType.Normal)]
    [HarmonyPatch(new Type[] {})]
    static class BallisticEffectOnCompleteMultifirePatch
    {
        private static Action<BallisticEffect> WeaponEffectOnComplete;
        public static bool Prepare()
        {
            if (!Core.ModSettings.BallisticNumberOfShots) return false;
            // build a call to WeaponEffect.OnComplete() so it can be called
            // a la base.OnComplete() from the context of a BallisticEffect
            var method = typeof(WeaponEffect).GetMethod("OnComplete", AccessTools.all);
            var dm = new DynamicMethod("WeaponEffectOnComplete", null, new Type[] {typeof(BallisticEffect)}, typeof(BallisticEffect));
            var gen = dm.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, method);
            gen.Emit(OpCodes.Ret);
            WeaponEffectOnComplete = (Action<BallisticEffect>) dm.CreateDelegate(typeof(Action<BallisticEffect>));
            return true;
        }

        static bool Prefix(ref int ___hitIndex, BallisticEffect __instance)
        {
            var damage = __instance.weapon.DamagePerShotAdjusted(__instance.weapon.parent.occupiedDesignMask);
            var magi = Traverse.Create(__instance);
            magi.Method("OnImpact", new object[] {damage}).GetValue();

            if (___hitIndex >= __instance.hitInfo.numberOfShots - 1)
            {
                WeaponEffectOnComplete(__instance);
                return false;
            }

            ___hitIndex++;
            __instance.Fire(__instance.hitInfo, ___hitIndex, 0);
            return false;
        }
    }

    [HarmonyPatch(typeof(LaserEffect), "OnComplete", MethodType.Normal)]
    [HarmonyPatch(new Type[] {})]
    static class LaserEffectOnCompleteMultifirePatch
    {
        public static bool Prepare()
        {
            return Core.ModSettings.LaserNumberOfShots;
        }

        static void Postfix(ref int ___hitIndex, LaserEffect __instance)
        {
            if (___hitIndex >= __instance.hitInfo.numberOfShots - 1) return;
            ___hitIndex++;
            __instance.Fire(__instance.hitInfo, ___hitIndex);
        }
    }


    // HAHA: patching a shot right into the end of Update didn't work
    // HAHA: patching weapon representation to call fire 3 times doesn't work
    
    // how-to?
    // if the laser has 1 shot, don't prefix shit
    // if the laser has more than one shot, then we need to find everywhere that interacts with the laser
    //   animation, and use arrays for each aspect
    // for laser color, we need to edit
    // before fire, setup more data structure like firing interval
    //
}