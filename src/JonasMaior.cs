using System.Collections.Generic;
using MoonCleaners;
using UnityEngine;

namespace BetterMoonSweeper
{
    // JONAS 2.0: o mesmo modelo do Jonas (MoonJoonasRig), só que maior. Só visual: a área limpa, a
    // velocidade e a capacidade vêm dos campos do robô (cleaningWidth, footprint, motion), não da escala.
    // Hipótese (não vista em jogo): o rig fica dentro do GameObject do MoonRobotVacuum e o pivô do robô
    // está no chão, então escalar a raiz do robô aumenta tudo sem afundar no terreno.
    public static class JonasMaior
    {
        public const float Escala = 1.5f;

        // Escala original de cada robô, lida uma vez: Setup rodar de novo não acumula a escala
        private static readonly Dictionary<int, Vector3> _original = new();
        private static bool _erroLogado;

        public static void Aplicar(MoonRobotVacuum robo)
        {
            try
            {
                if (robo == null) return;
                var t = robo.transform;
                int id = robo.GetInstanceID();
                if (!_original.TryGetValue(id, out var original))
                {
                    original = t.localScale;
                    _original[id] = original;
                }
                var alvo = original * Escala;
                if (t.localScale == alvo) return;
                t.localScale = alvo;
                BetterMoonSweeperMain.Log.LogInfo($"JONAS 2.0: {t.name} escalado {Escala}x.");
            }
            catch (System.Exception e)
            {
                if (_erroLogado) return;
                _erroLogado = true;
                BetterMoonSweeperMain.Log.LogError($"JONAS 2.0 falhou: {e}");
            }
        }
    }
}
