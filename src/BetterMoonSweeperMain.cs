using MelonLoader;

[assembly: MelonInfo(typeof(BetterMoonSweeper.BetterMoonSweeperMain), "BetterMoonSweeper", BetterMoonSweeper.BetterMoonSweeperMain.Version, "Diuli")]
[assembly: MelonGame("DragDropGames", "MoonSweepers")]

namespace BetterMoonSweeper
{
    public class BetterMoonSweeperMain : MelonMod
    {
        // Precisa ser igual em todos os jogadores do lobby (quem tiver versão diferente é bloqueado)
        public const string Version = "0.2.0";

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg($"BetterMoonSweeper {Version} carregado.");
            // Os patches Harmony de src/Patches.cs são aplicados automaticamente pelo MelonLoader.
            // TODO fase 1: bloquear no lobby quem não tem o mod, e guardar os níveis do mod fora do save original
        }

        public override void OnUpdate() => GravityBootsHotkey.Tick();
    }
}
