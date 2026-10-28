using MoonCleaners;

namespace BetterMoonSweeper
{
    // Bigger Catcher (spec 006). CatcherStoreKg, GarageSiloCapacityKg e BeltSpeed são constantes do jogo:
    // ler funciona, gravar derruba o jogo (AccessViolation na 0.5.0). Então o mod não escreve nelas.
    // Cada receptor de cápsulas tem um armazenamento em MoonWorld.vehicleStorage, com a capacidade
    // própria em MoonVehicleStorageSnapshot.capacity; é essa capacidade (dado normal, gravável) que sobe.
    public static class Armazenamento
    {
        private static string _prefixo;
        private static bool _falhou;
        private static int _aplicados = -1;

        // Capacidade que cada receptor deve ter agora (100 × nível). A UI do jogo usa a constante 100
        public static double Capacidade = double.NaN;

        public static void Aplicar(MoonWorld world)
        {
            if (_falhou || world == null) return;
            try
            {
                var storages = world.vehicleStorage;
                if (storages == null) return;
                _prefixo ??= PrefixoDosReceptores();
                if (_prefixo.Length < 3) // prefixo curto demais casaria com outros armazenamentos
                    throw new System.InvalidOperationException($"prefixo dos receptores inesperado: \"{_prefixo}\"");

                double alvo = MoonWorld.CatcherStoreKg * Upgrades.ReceptorGrande.Multiplicador(FactoryTree.Nivel(world, Upgrades.ReceptorGrande));
                Capacidade = alvo;
                int total = 0;
                foreach (var par in storages)
                {
                    if (!par.Key.StartsWith(_prefixo, System.StringComparison.Ordinal)) continue;
                    var s = par.Value;
                    if (s == null) continue;
                    total++;
                    if (System.Math.Abs(s.capacity - alvo) > 1e-6)
                    {
                        BetterMoonSweeperMain.Log.LogInfo($"Receptor {par.Key}: capacidade {s.capacity} → {alvo} kg");
                        s.capacity = alvo;
                    }
                }
                if (total != _aplicados)
                {
                    _aplicados = total;
                    BetterMoonSweeperMain.Log.LogInfo($"Receptores de cápsulas encontrados: {total} (prefixo \"{_prefixo}\", capacidade {alvo} kg)");
                }
            }
            catch (System.Exception e)
            {
                _falhou = true;
                BetterMoonSweeperMain.Log.LogError($"Bigger Catcher: falha; o mod não vai tentar de novo: {e}");
            }
        }

        // Parte comum dos ids dos receptores 1 e 2 (o que vem antes do número do receptor)
        private static string PrefixoDosReceptores()
        {
            string a = MoonWorld.CatcherStoragePrefix(1), b = MoonWorld.CatcherStoragePrefix(2);
            int n = 0;
            while (n < a.Length && n < b.Length && a[n] == b[n]) n++;
            return a.Substring(0, n);
        }
    }
}
