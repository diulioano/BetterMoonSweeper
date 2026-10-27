using MelonLoader;

[assembly: MelonInfo(typeof(JonasMod.JonasModMain), "JonasMod", JonasMod.JonasModMain.Version, "Diuli")]
[assembly: MelonGame("DragDropGames", "MoonSweepers")]

namespace JonasMod
{
    public class JonasModMain : MelonMod
    {
        // Precisa ser igual em todos os jogadores do lobby (quem tiver versão diferente é bloqueado)
        public const string Version = "0.1.0";

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg($"JonasMod {Version} carregado.");
            // Os patches Harmony de src/Patches.cs são aplicados automaticamente pelo MelonLoader.
            // TODO fase 1: bloquear no lobby quem não tem o mod, e guardar os níveis do mod fora do save original
        }
    }
}
