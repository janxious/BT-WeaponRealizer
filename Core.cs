using System;
using System.Reflection;
using Harmony;
using Newtonsoft.Json;

namespace WeaponRealizer
{
    public static partial class Core
    {
        public const string ModName = "WeaponRealizer";
        public const string ModId   = "com.joelmeador.WeaponRealizer";

        internal static Settings ModSettings = new Settings();
        internal static string ModDirectory;

        public static void Init(string directory, string settingsJson)
        {
            ModDirectory = directory;
            try
            {
                ModSettings = JsonConvert.DeserializeObject<Settings>(settingsJson);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                ModSettings = new Settings();
            }
            HarmonyInstance.DEBUG = ModSettings.debug;
            var harmony = HarmonyInstance.Create(ModId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Patches.Apply(harmony);
        }
    }
}