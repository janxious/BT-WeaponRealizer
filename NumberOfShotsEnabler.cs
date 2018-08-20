using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
}