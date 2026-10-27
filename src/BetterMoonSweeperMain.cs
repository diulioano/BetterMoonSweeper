using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace BetterMoonSweeper
{
    [BepInPlugin(Guid, "BetterMoonSweeper", Version)]
    public class BetterMoonSweeperMain : BasePlugin
    {
        public const string Guid = "com.diuli.bettermoonsweeper";

        // Precisa ser igual em todos os jogadores do lobby (quem tiver versão diferente é bloqueado)
        public const string Version = "0.5.0";

        // Cópia estática do Log do plugin, para as classes estáticas do mod usarem
        public static new ManualLogSource Log;

        public override void Load()
        {
            Log = base.Log;
            Log.LogInfo($"BetterMoonSweeper {Version} carregado.");
            // O BepInEx não aplica os patches sozinho: aplica todos os [HarmonyPatch] de src/Patches.cs
            new Harmony(Guid).PatchAll(typeof(BetterMoonSweeperMain).Assembly);
            // TODO fase 1: bloquear no lobby quem não tem o mod, e guardar os níveis do mod fora do save original
        }
    }
}
