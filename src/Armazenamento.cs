using MoonCleaners;

namespace BetterMoonSweeper
{
    // Upgrades de armazenamento (spec 006): reescreve as capacidades estáticas do jogo como
    // base × multiplicador do nível, do mesmo jeito que os tempos de ciclo em StepProductionPatch.
    public static class Armazenamento
    {
        private sealed class Capacidade
        {
            public string Nome;
            public UpgradeDef Upgrade;
            public System.Func<double> Ler;
            public System.Action<double> Gravar;
            public double Base = -1;
            public bool Falhou;
        }

        private static readonly Capacidade[] Capacidades =
        {
            new() { Nome = "CatcherStoreKg", Upgrade = Upgrades.ReceptorGrande,
                    Ler = () => MoonWorld.CatcherStoreKg, Gravar = v => MoonWorld.CatcherStoreKg = v },
            new() { Nome = "GarageSiloCapacityKg", Upgrade = Upgrades.SiloGrande,
                    Ler = () => MoonWorld.GarageSiloCapacityKg, Gravar = v => MoonWorld.GarageSiloCapacityKg = v },
            new() { Nome = "MoonCollector.Capacity", Upgrade = Upgrades.EstacaoGrande,
                    Ler = () => MoonCollector.Capacity, Gravar = v => MoonCollector.Capacity = v },
            // Não é armazenamento, mas segue a mesma regra base × nível (velocidade das esteiras)
            new() { Nome = "BeltSpeed", Upgrade = Upgrades.EsteiraTurbo,
                    Ler = () => MoonWorld.BeltSpeed, Gravar = v => MoonWorld.BeltSpeed = (float)v },
        };

        public static void Aplicar(MoonWorld world)
        {
            foreach (var c in Capacidades)
            {
                if (c.Falhou) continue;
                try
                {
                    if (c.Base < 0)
                    {
                        c.Base = c.Ler(); // valor original do jogo, lido uma vez
                        BetterMoonSweeperMain.Log.LogInfo($"Capacidade {c.Nome}: base={c.Base}");
                    }
                    double alvo = c.Base * c.Upgrade.Multiplicador(FactoryTree.Nivel(world, c.Upgrade));
                    // Tolerância: BeltSpeed é float, e a leitura de volta não é idêntica ao double gravado
                    if (System.Math.Abs(c.Ler() - alvo) > System.Math.Abs(alvo) * 1e-5)
                    {
                        c.Gravar(alvo);
                        BetterMoonSweeperMain.Log.LogInfo($"Capacidade {c.Nome}: {c.Base} → {alvo}");
                    }
                }
                catch (System.Exception e)
                {
                    c.Falhou = true;
                    BetterMoonSweeperMain.Log.LogError($"Capacidade {c.Nome} não pôde ser ajustada; o mod não vai tentar de novo: {e}");
                }
            }
        }
    }
}
