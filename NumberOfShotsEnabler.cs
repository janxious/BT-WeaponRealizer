using System;
using System.Collections.Generic;
using Harmony;

namespace WeaponRealizer
{
    public static class NumberOfShotsEnabler
    {
        static readonly Dictionary<int, int> _shotCountHolder = new Dictionary<int, int>();

        static void BallisticEffectUpdatePrefix(BallisticEffect __instance)
        {
            if (__instance.currentState == WeaponEffect.WeaponEffectState.Complete) return;
            try
            {
                var ballisticEffect = __instance;
                var instance = Traverse.Create(ballisticEffect);
                var effectId = ballisticEffect.GetInstanceID();

                var allBullets = instance.Method("AllBulletsComplete").GetValue<bool>();

//                if (ballisticEffect.currentState != WeaponEffect.WeaponEffectState.Complete &&
//                    _shotCountHolder.ContainsKey(effectId))
//                    Logger.Debug($"effectid: {effectId}\n" +
//                                 $"all bullets? {allBullets}\n" +
//                                 $"_shotCountHolder[effectId]: {_shotCountHolder[effectId]}\n" +
//                                 $"ballisticEffect.hitInfo.numberOfShots: {ballisticEffect.hitInfo.numberOfShots}");


                if (!_shotCountHolder.ContainsKey(effectId))
                {
                    _shotCountHolder[effectId] = 1;
                    Logger.Debug($"effectId: shotcount for {effectId} added");
                }

                if ((_shotCountHolder.ContainsKey(effectId) &&
                     _shotCountHolder[effectId] != ballisticEffect.hitInfo.numberOfShots) ||
                    ballisticEffect.currentState != WeaponEffect.WeaponEffectState.WaitingForImpact ||
                    !allBullets) return;

                var hitIndex = instance.Field("hitIndex");
                Logger.Debug($"hitIndex before: {hitIndex.GetValue<int>()}");
                instance.Field("hitIndex").SetValue(_shotCountHolder[effectId] - 1);
                Logger.Debug($"hitIndex after: {hitIndex.GetValue<int>()}");
                var damage = ballisticEffect.weapon.DamagePerShotAdjusted(ballisticEffect.weapon.parent.occupiedDesignMask);
                if (_shotCountHolder[effectId] >= ballisticEffect.hitInfo.numberOfShots)
                {
                    _shotCountHolder[effectId] = 1;
                    instance.Method("OnImpact", new object[] {damage}).GetValue();
                    Logger.Debug($"effectId: {effectId} shotcount reset");
                }
                else
                {
                    _shotCountHolder[effectId]++;
                    instance.Method("OnImpact", new object[] {damage}).GetValue();
                    ballisticEffect.Fire(ballisticEffect.hitInfo, 0, 0);
                    Logger.Debug($"effectId: {effectId} shotcount incremented to: {_shotCountHolder[effectId]}");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        static void BallisticEffectOnCompletePrefix(BallisticEffect __instance, ref float __state)
        {
            try
            {
                Logger.Debug("Setting damagepershot to zero");
                var weapon = __instance.weapon;
                __state = weapon.DamagePerShot;
                weapon.StatCollection.Set("DamagePerShot", 0f);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        static void BallisticEffectOnCompletePostfix(BallisticEffect __instance, float __state)
        {
            try
            {
                Logger.Debug($"Setting damagepershot back to {__state}");
                __instance.weapon.StatCollection.Set("DamagePerShot", __state);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}