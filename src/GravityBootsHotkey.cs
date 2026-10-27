using Il2CppMoonCleaners;
using MelonLoader;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace BetterMoonSweeper
{
    // Ctrl (esquerdo ou direito) liga/desliga a bota de gravidade do jogador local.
    // Usa o método nativo ToggleGravityBoots, que cuida de som e sincronização (specs/002, research R1).
    public static class GravityBootsHotkey
    {
        private static KeyControl _ctrlEsquerdo;
        private static KeyControl _ctrlDireito;
        private static bool _estavaEmPartida;
        private static bool _erroLogado;

        public static void Tick()
        {
            try { Verificar(); }
            catch (System.Exception e)
            {
                // Loga só a primeira vez, para não inundar o log se um símbolo mudar numa atualização do jogo
                if (_erroLogado) return;
                _erroLogado = true;
                Melon<BetterMoonSweeperMain>.Logger.Error($"Atalho da bota de gravidade falhou: {e}");
            }
        }

        private static void Verificar()
        {
            bool emPartida = MoonSession.InGame;
            bool novaPartida = emPartida && !_estavaEmPartida;
            _estavaEmPartida = emPartida;
            if (!emPartida) return;

            // As teclas ficam em cache: cada leitura de PlayKeyboard cria um wrapper IL2CPP novo
            if (_ctrlEsquerdo == null || novaPartida)
            {
                var teclado = MoonInput.PlayKeyboard ?? Keyboard.current;
                if (teclado == null) return;
                _ctrlEsquerdo = teclado.leftCtrlKey;
                _ctrlDireito = teclado.rightCtrlKey;
#if DIAGNOSTICO
                LogarBindingsComCtrl();
#endif
            }
            if (!_ctrlEsquerdo.wasPressedThisFrame && !_ctrlDireito.wasPressedThisFrame) return;

            // Hipótese (research R4): MenuOpen cobre a pausa e a árvore de upgrades
            if (MoonInput.Typing || MoonFramePacing.MenuOpen()) return;

            var walker = MoonRunOver.LocalWalker;
            if (walker == null || !walker.OwnsGravityBoots) return;
            // Hipótese (research R4): o jogo usa MenuBlocked/UIBlocked para travar o input com UI aberta
            if (walker.MenuBlocked || walker.UIBlocked) return;
            if (walker.Mounted || walker.Passenger || walker.Riding != null || walker.Building || MoonInput.BuildToolInHand) return;
            if (walker.Knocked || walker.Climbing || walker.Resting || walker.Travelling || walker.Held || walker.Flying) return;

            var log = Melon<BetterMoonSweeperMain>.Logger;
            if (walker.ToggleGravityBoots())
                log.Msg($"Bota de gravidade: {(walker.GravityBootsOn ? "ligada" : "desligada")} (Ctrl)");
            else
                log.Msg("ToggleGravityBoots retornou false");
        }

#if DIAGNOSTICO
        private static bool _diagnosticoFeito;

        // Confere se alguma tecla do jogo já usa Ctrl (research R6). Roda uma vez, na primeira partida.
        private static void LogarBindingsComCtrl()
        {
            if (_diagnosticoFeito) return;
            _diagnosticoFeito = true;
            var log = Melon<BetterMoonSweeperMain>.Logger;
            int usamCtrl = 0;
            foreach (var prop in typeof(MoonInput).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            {
                if (prop.PropertyType != typeof(MoonInput.Binding)) continue;
                var keys = (prop.GetValue(null) as MoonInput.Binding)?.keys;
                if (keys == null) continue;
                foreach (var k in keys)
                    if (k == Key.LeftCtrl || k == Key.RightCtrl)
                    {
                        log.Msg($"[diagnóstico] MoonInput.{prop.Name} usa {k}");
                        usamCtrl++;
                    }
            }
            log.Msg($"[diagnóstico] Bindings com Ctrl: {usamCtrl}");
        }
#endif
    }
}
