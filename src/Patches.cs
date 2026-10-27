using HarmonyLib;
using Il2CppMoonCleaners;

namespace JonasMod
{
    // Garante que os nós do mod existam antes da tela da árvore ser montada
    [HarmonyPatch(typeof(MoonUpgradeTreeUI), nameof(MoonUpgradeTreeUI.Build))]
    internal static class TreeUIBuildPatch
    {
        private static void Prefix() => FactoryTree.Injetar();
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
}
