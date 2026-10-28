using HarmonyLib;
using MoonCleaners;

namespace BetterMoonSweeper
{
    // Garante que os nós do mod existam antes da tela da árvore ser montada
    [HarmonyPatch(typeof(MoonUpgradeTreeUI), nameof(MoonUpgradeTreeUI.Build))]
    internal static class TreeUIBuildPatch
    {
        private static void Prefix() => FactoryTree.Injetar();
    }

    // Chama o atalho Ctrl da bota de gravidade uma vez por quadro durante a partida.
    // Evita AddComponent, que depende de class injection (specs/004, research R3)
    [HarmonyPatch(typeof(MoonWorld), nameof(MoonWorld.Update))]
    internal static class WorldUpdatePatch
    {
        private static void Postfix() => GravityBootsHotkey.Tick();
    }

    // JONAS 2.0: aumenta o Jonas quando o jogo monta o rig dele
    [HarmonyPatch(typeof(MoonJoonasRig), nameof(MoonJoonasRig.Setup))]
    internal static class JoonasSetupPatch
    {
        private static void Postfix(MoonRobotVacuum owner) => JonasMaior.Aplicar(owner);
    }

    // Acelera o compactador e a prensa de pedra ajustando o tempo de ciclo das specs estáticas
    // antes de cada passo de produção. Todos os jogadores têm o mod e os mesmos níveis, então host e
    // clientes calculam o mesmo tempo.
    [HarmonyPatch(typeof(MoonWorld), nameof(MoonWorld.StepProduction))]
    internal static class StepProductionPatch
    {
        private static float _baseCompactador = -1f;
        private static float _basePrensa = -1f;

        private static void Prefix(MoonWorld __instance)
        {
            FactoryTree.Injetar();
            Ajustar(MoonWorld.ChamberSpec, ref _baseCompactador, __instance, Upgrades.CompactadorTurbo);
            Ajustar(MoonWorld.PressSpec, ref _basePrensa, __instance, Upgrades.PrensaTurbo);
            Armazenamento.Aplicar(__instance);
        }

        private static void Ajustar(MoonWorld.ProcessorSpec spec, ref float baseSeconds, MoonWorld world, UpgradeDef def)
        {
            if (spec == null) return;
            if (baseSeconds < 0f) baseSeconds = spec.cycleSeconds; // valor original do jogo, lido uma vez
            float alvo = baseSeconds / def.Multiplicador(FactoryTree.Nivel(world, def));
            if (spec.cycleSeconds != alvo) spec.cycleSeconds = alvo;
        }
    }

    // Contrato Lunar: multiplica o multiplicador de pagamento que o jogo já aplica
    [HarmonyPatch(typeof(MoonWorld), nameof(MoonWorld.PayoutMultiplier))]
    internal static class PayoutMultiplierPatch
    {
        private static void Postfix(MoonWorld __instance, ref Il2CppSystem.Decimal __result)
        {
            int nivel = FactoryTree.Nivel(__instance, Upgrades.ContratoLunar);
            if (nivel <= 0) return;
            var extra = (Il2CppSystem.Decimal)(double)Upgrades.ContratoLunar.Multiplicador(nivel);
            __result = __result * extra;
        }
    }

    // Turbo Belts: BeltSpeed é constante do jogo (gravar derruba o jogo), então o tempo que as esteiras
    // andam é que aumenta: StepConveyors recebe dt × multiplicador do nível
    [HarmonyPatch(typeof(MoonWorld), nameof(MoonWorld.StepConveyors))]
    internal static class StepConveyorsPatch
    {
        private static bool _avisou;

        private static void Prefix(MoonWorld __instance, ref float dt)
        {
            int nivel = FactoryTree.Nivel(__instance, Upgrades.EsteiraTurbo);
            if (nivel <= 0) return;
            dt *= Upgrades.EsteiraTurbo.Multiplicador(nivel);
            if (!_avisou)
            {
                _avisou = true;
                BetterMoonSweeperMain.Log.LogInfo($"Turbo Belts: esteiras ×{Upgrades.EsteiraTurbo.Multiplicador(nivel)} (nível {nivel}).");
            }
        }
    }
}
