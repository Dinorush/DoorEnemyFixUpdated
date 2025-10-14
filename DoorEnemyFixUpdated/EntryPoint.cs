using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace DoorEnemyFixUpdated
{
    // Using DoorEnemyFix GUID to override it
    [BepInPlugin("Localia." + MODNAME, MODNAME, "1.1.1")]
    internal sealed class EntryPoint : BasePlugin
    {
        public const string MODNAME = "DoorEnemyFix";

        public override void Load()
        {
            new Harmony(MODNAME).PatchAll();
            Log.LogMessage("Loaded " + MODNAME);
        }
    }
}