using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using BattleTech;
using BattleTech.Save;
using BattleTech.Save.SaveGameStructure;
using Harmony;

namespace WeaponRealizer
{
    public static class NumberOfShotsEnabler
    {
        private static Dictionary<int, int> _shotCountHolder = new Dictionary<int, int>();

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal static void BallisticEffectUpdatePrefix(BallisticEffect __instance, ref int ___hitIndex) {
            try {
                var ballisticEffect = __instance;
                if (ballisticEffect.currentState == WeaponEffect.WeaponEffectState.Complete) return;

                var instance = Traverse.Create(ballisticEffect);
                var allBullets = instance.Method("AllBulletsComplete").GetValue<bool>();

                if (ballisticEffect.currentState != WeaponEffect.WeaponEffectState.WaitingForImpact || !allBullets) return;

                var effectId = ballisticEffect.GetInstanceID();
                if (!_shotCountHolder.ContainsKey(effectId))
                {
                    _shotCountHolder[effectId] = 1;
                    Logger.Debug($"shotcount for effectId {effectId} added");
                }

                ___hitIndex = _shotCountHolder[effectId] - 1;
                var damage = ballisticEffect.weapon.DamagePerShotAdjusted(ballisticEffect.weapon.parent.occupiedDesignMask);
                if (_shotCountHolder[effectId] >= ballisticEffect.hitInfo.numberOfShots) {
                    _shotCountHolder[effectId] = 1;
                    instance.Method("OnImpact", new object[] {damage}).GetValue();
                    Logger.Debug($"effectId: {effectId} shotcount reset");
                }
                else {
                    _shotCountHolder[effectId]++;
                    instance.Method("OnImpact", new object[] {damage}).GetValue();
                    ballisticEffect.Fire(
                        hitInfo: ballisticEffect.hitInfo, 
                        hitIndex: ___hitIndex, // TODO: setting this to zero acts as a sort of "super TAG"
                        emitterIndex: 0
                    );
                    Logger.Debug($"effectId: {effectId} shotcount incremented to:{_shotCountHolder[effectId]}");
                }
            }
            catch (Exception e) {
                Logger.Error(e);
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal static void BallisticEffectOnCompletePrefix(BallisticEffect __instance, ref float __state)
        {
            try {
                Logger.Debug("BallisticEffectOnCompletePrefix");
                var weapon = __instance.weapon;
                __state = weapon.DamagePerShot;
                weapon.StatCollection.Set<float>("DamagePerShot", 0);
            }
            catch (Exception e) {
                Logger.Error(e);
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        internal static void BallisticEffectOnCompletePostfix(BallisticEffect __instance, float __state)
        {
            try
            {
                Logger.Debug("BallisticEffectOnCompletePostfix");
                __instance.weapon.StatCollection.Set("DamagePerShot", __state);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }

//    [HarmonyPatch(typeof(LaserEffect), "OnComplete", MethodType.Normal)]
//    static class LaserEffectOnCompletePatcher
//    {
//        [SuppressMessage("ReSharper", "InconsistentNaming")]
//        internal static void LaserEffectOnCompletePrefix(LaserEffect __instance, ref float __state)
//        {
//            try {
//                Logger.Debug("BallisticEffectOnCompletePrefix");
//                var weapon = __instance.weapon;
//                __state = weapon.DamagePerShot;
//                weapon.StatCollection.Set<float>("DamagePerShot", 0);
//            }
//            catch (Exception e) {
//                Logger.Error(e);
//            }
//        }
//
//        [SuppressMessage("ReSharper", "InconsistentNaming")]
//        internal static void LaserEffectOnCompletePostfix(LaserEffect __instance, float __state)
//        {
//            try
//            {
//                Logger.Debug("BallisticEffectOnCompletePostfix");
//                __instance.weapon.StatCollection.Set("DamagePerShot", __state);
//            }
//            catch (Exception e)
//            {
//                Logger.Error(e);
//            }
//        }
//    }
//    [HarmonyPatch(typeof(WeaponEffect), "Update", new Type[] { })]
//    static class WeaponEffectUpdateLogger
//    {
//        static void Prefix(WeaponEffect __instance)
//        {
//            if (LaserEffectBaseUpdateCallTest.WriteWeaponEffectLog && __instance.currentState == WeaponEffect.WeaponEffectState.Firing)
//                Logger.Debug("Hello from WeaponEffectUpdate");
//            LaserEffectBaseUpdateCallTest.CallShortCutter++;
//        }
//    }
//
//    [HarmonyPatch(typeof(LaserEffect), "Update", new Type[] { })]
//    static class LaserEffectBaseUpdateCallTest
//    {
//        public static int CallShortCutter = 0;
//        public static bool WriteWeaponEffectLog = false;
//        private static Dictionary<int, int> _shotCountHolder = new Dictionary<int, int>();
//
//        private static Action<LaserEffect> WeaponEffectUpdate = null;
//
//        static bool Prepare()
//        {
//            var method = typeof(WeaponEffect).GetMethod("Update", AccessTools.all);
//            var dm = new DynamicMethod("WeaponEffectUpdate", null, new Type[] {typeof(LaserEffect)},
//                typeof(LaserEffect));
//            var gen = dm.GetILGenerator();
//            gen.Emit(OpCodes.Ldarg_0);
//            gen.Emit(OpCodes.Call, method);
//            gen.Emit(OpCodes.Ret);
//
//            WeaponEffectUpdate = (Action<LaserEffect>) dm.CreateDelegate(typeof(Action<LaserEffect>));
//            return true;
//        }

//        static bool Prefix(LaserEffect __instance)
//        {
//            if (__instance.currentState != WeaponEffect.WeaponEffectState.Firing) return true;
//            Logger.Debug($"Trying to call base update! {CallShortCutter}");
//            CallShortCutter++;
//            WriteWeaponEffectLog = true;
//            try
//            {
//                WeaponEffectUpdate(__instance);
//                Logger.Debug("we did an update and it didn't crash");
//            }
//            catch (Exception e)
//            {
//                Logger.Error(e);
//                throw;
//            }
//            finally
//            {
//                WriteWeaponEffectLog = false;
//            }
//            return true;
//        }

//        static void Prefix(LaserEffect __instance, ref int ___hitIndex)
//        {
//            try
//            {
//                var laserEffect = __instance;
//                if (laserEffect.currentState == WeaponEffect.WeaponEffectState.Complete) return;
//
//                var instance = Traverse.Create(laserEffect);
//                if (laserEffect.currentState != WeaponEffect.WeaponEffectState.WaitingForImpact) return;
//
//                var effectId = laserEffect.GetInstanceID();
//                if (!_shotCountHolder.ContainsKey(effectId))
//                {
//                    _shotCountHolder[effectId] = 1;
//                    Logger.Debug($"shotcount for effectId {effectId} added");
//                }
//
//                ___hitIndex = _shotCountHolder[effectId] - 1;
//                var damage = laserEffect.weapon.DamagePerShotAdjusted(laserEffect.weapon.parent.occupiedDesignMask);
//                if (_shotCountHolder[effectId] >= laserEffect.hitInfo.numberOfShots)
//                {
//                    _shotCountHolder[effectId] = 1;
//                    instance.Method("OnImpact", new object[] {damage}).GetValue();
//                    Logger.Debug($"effectId: {effectId} shotcount reset");
//                }
//                else
//                {
//                    _shotCountHolder[effectId]++;
//                    instance.Method("OnImpact", new object[] {damage}).GetValue();
//                    laserEffect.Fire(
//                        hitInfo: laserEffect.hitInfo,
//                        hitIndex: ___hitIndex, // TODO: setting this to zero acts as a sort of "super TAG"
//                        emitterIndex: 0
//                    );
//                    Logger.Debug($"effectId: {effectId} shotcount incremented to:{_shotCountHolder[effectId]}");
//                }
//            }
//            catch (Exception e)
//            {
//                Logger.Error(e);
//            }
//        }
//    }
}