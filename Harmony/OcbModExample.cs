using BepInEx;
using HarmonyLib;
using System.Reflection;
using BepInEx.Logging;
using System.Linq;

namespace OcbModExample
{

    [BepInPlugin("ch.ocbnet.example", "OcbModExample", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        private void Awake()
        {
            Log = base.Logger;
            // Enable this to run the IL code instead of using burst compiler
            // Will make the game much slower, but can help to figure out patches
            // Once you know what to do, you may be able to use a transpiler instead
            // Unity.Burst.BurstCompiler.Options.EnableBurstCompilation = false;
            var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "ch.ocbnet.example");
            var patchedMethods = harmony.GetPatchedMethods().ToArray();
            log.info("Plugin OcbModExample is loaded! Patched {0} methods", patchedMethods.Length);
            foreach (var patchedMethod in patchedMethods)
            {
                log.info("Patched method: {0}:{1}", patchedMethod.Module.Name, patchedMethod.Name);
            }
        }
    }

    // Little logging helper
    public static class log
    {
        public static void fatal(string msg) =>
            Plugin.Log.Log(LogLevel.Fatal, msg);
        public static void error(string msg) =>
            Plugin.Log.Log(LogLevel.Error, msg);
        public static void warn(string msg) =>
            Plugin.Log.Log(LogLevel.Warning, msg);
        public static void message(string msg) =>
            Plugin.Log.Log(LogLevel.Message, msg);
        public static void info(string msg) =>
            Plugin.Log.Log(LogLevel.Info, msg);
        public static void debug(string msg) =>
            Plugin.Log.Log(LogLevel.Debug, msg);
        public static void fatal(string fmt, params object[] args) =>
            Plugin.Log.Log(LogLevel.Fatal, string.Format(fmt, args));
        public static void error(string fmt, params object[] args) =>
            Plugin.Log.Log(LogLevel.Error, string.Format(fmt, args));
        public static void warn(string fmt, params object[] args) =>
            Plugin.Log.Log(LogLevel.Warning, string.Format(fmt, args));
        public static void message(string fmt, params object[] args) =>
            Plugin.Log.Log(LogLevel.Message, string.Format(fmt, args));
        public static void info(string fmt, params object[] args) =>
            Plugin.Log.Log(LogLevel.Info, string.Format(fmt, args));
        public static void debug(string fmt, params object[] args) =>
            Plugin.Log.Log(LogLevel.Debug, string.Format(fmt, args));
    }

    // Harmony patch to change post vans to trucks and vice-versa
    [HarmonyPatch(typeof(Game.Prefabs.PostFacility), "Initialize")]
    static class PostFacility_Patch
    {
        static void Prefix(Game.Prefabs.PostFacility __instance)
        {
            #pragma warning disable CS0168 // Disable warning
            // Only to show-case that we can now use publicized stuff
            // With original Game.dll reference, this would error
            Game.Simulation.PostFacilityAISystem.PostFacilityTickJob Job;
            #pragma warning restore CS0168 // Re-enable warning
        }
    }

}
