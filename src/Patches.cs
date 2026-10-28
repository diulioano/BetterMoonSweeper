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

    // Os níveis dos nós do mod ficam no save do jogo. Se a restauração do save rodar antes dos nós
    // existirem na árvore, o jogo pode descartar os níveis de ids que ele não conhece (níveis "resetados").
    // Por isso os nós são criados antes de restaurar e de aplicar compras vindas do host.
    [HarmonyPatch(typeof(MoonWorld), nameof(MoonWorld.RestoreUpgradeTree))]
    internal static class RestoreUpgradeTreePatch
    {
        private static void Prefix() => FactoryTree.Injetar();
    }

    [HarmonyPatch(typeof(MoonWorld), nameof(MoonWorld.CrewPurchase))]
    internal static class CrewPurchasePatch
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

    // O painel que aparece ao olhar um receptor de cápsulas calcula "kg / 100 kg" e o medidor com a
    // constante do jogo. Durante MoonCatcherLook.Show, trocamos o 100 pela capacidade do nível.
    [HarmonyPatch(typeof(MoonCatcherLook), nameof(MoonCatcherLook.Show))]
    internal static class CatcherLookShowPatch
    {
        internal static bool Dentro;
        internal static double Kg;

        private static void Prefix(MoonWorld world, int id)
        {
            Dentro = true;
            try { Kg = world.CatcherKilograms(id); } catch { Kg = double.NaN; }
        }

        private static void Finalizer() => Dentro = false; // roda mesmo se Show lançar exceção
    }

    [HarmonyPatch(typeof(MoonLookPanel), nameof(MoonLookPanel.SetRow))]
    internal static class CatcherLookRowPatch
    {
        private static int _logs;

        private static void Prefix(string label, ref float fill, ref string value)
        {
            if (!CatcherLookShowPatch.Dentro) return;
            try
            {
                double cap = Armazenamento.Capacidade;
                double antigo = MoonWorld.CatcherStoreKg;
                if (_logs++ < 6)
                    BetterMoonSweeperMain.Log.LogInfo($"Painel do receptor: \"{label}\" fill={fill} value=\"{value}\" (kg={CatcherLookShowPatch.Kg}, capacidade={cap})");
                if (double.IsNaN(cap) || cap == antigo || double.IsNaN(CatcherLookShowPatch.Kg)) return;
                if (System.Math.Abs(fill - CatcherLookShowPatch.Kg / antigo) > 0.02 && !(fill >= 0.999f && CatcherLookShowPatch.Kg >= antigo)) return;
                fill = (float)System.Math.Min(1.0, CatcherLookShowPatch.Kg / cap);
                if (value != null) value = value.Replace(MoonCatcherLook.Kg(antigo), MoonCatcherLook.Kg(cap));
            }
            catch (System.Exception e)
            {
                BetterMoonSweeperMain.Log.LogError($"Painel do receptor: {e.Message}");
            }
        }
    }
}
