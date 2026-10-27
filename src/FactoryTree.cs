using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMoonCleaners;
using MelonLoader;
using UnityEngine;

namespace JonasMod
{
    // Injeta o ramo "Fábrica" e os nós do mod na árvore de upgrades do jogo.
    // Os nós passam a usar a compra, o save e a sincronização nativos (TryPurchaseTreeNode / CrewPurchase).
    public static class FactoryTree
    {
        // Se a tela não aceitar um 7º ramo, mude para false e os nós vão para o ramo Economy
        public static bool UsarRamoProprio = true;

        public const string NomeDoRamo = "Fábrica";
        public const string SubtituloDoRamo = "Máquinas mais rápidas, mais lucro.";
        public static readonly Color CorDoRamo = new(1f, 0.55f, 0.15f);

        private static bool _injetado;

        public static MoonUpgradeTree.Branch Ramo =>
            UsarRamoProprio ? (MoonUpgradeTree.Branch)MoonUpgradeTree.BranchNames.Length - (_injetado ? 1 : 0)
                            : MoonUpgradeTree.Branch.Economy;

        public static void Injetar()
        {
            if (_injetado) return;

            var nodes = MoonUpgradeTree.Nodes;
            if (nodes == null) return; // árvore ainda não montada pelo jogo
            foreach (var n in nodes)
                if (n.id == Upgrades.CompactadorTurbo.NodeId) { _injetado = true; return; }

            var ramo = Ramo;
            if (UsarRamoProprio)
            {
                MoonUpgradeTree.BranchNames = Append(MoonUpgradeTree.BranchNames, NomeDoRamo);
                MoonUpgradeTree.BranchTaglines = Append(MoonUpgradeTree.BranchTaglines, SubtituloDoRamo);
                MoonUpgradeTree.BranchColors = Append(MoonUpgradeTree.BranchColors, CorDoRamo);
            }

            var novos = new MoonUpgradeTree.Node[nodes.Length + Upgrades.Todos.Length];
            for (int i = 0; i < nodes.Length; i++) novos[i] = nodes[i];
            for (int i = 0; i < Upgrades.Todos.Length; i++)
            {
                var node = CriarNo(Upgrades.Todos[i], ramo, i);
                novos[nodes.Length + i] = node;
                MoonUpgradeTree.byId?.TryAdd(node.id, node);
            }
            MoonUpgradeTree.Nodes = new Il2CppReferenceArray<MoonUpgradeTree.Node>(novos);

            _injetado = true;
            Melon<JonasModMain>.Logger.Msg($"Árvore: {Upgrades.Todos.Length} nós injetados no ramo {(int)ramo} ({(UsarRamoProprio ? NomeDoRamo : "Economy")}).");
        }

        private static MoonUpgradeTree.Node CriarNo(UpgradeDef def, MoonUpgradeTree.Branch ramo, int indice)
        {
            var precos = new int[def.NivelMaximo];
            for (int n = 1; n <= def.NivelMaximo; n++) precos[n - 1] = def.PrecoDoNivel(n);

            System.Func<int, string> efeito = nivel =>
            {
                float m = def.Multiplicador(nivel);
                return def.Id == UpgradeId.ContratoLunar
                    ? $"+{(m - 1f) * 100f:0}% de dinheiro"
                    : $"{m:0.#}x mais rápido";
            };

            return new MoonUpgradeTree.Node
            {
                id = def.NodeId,
                title = def.Nome,
                summary = def.Resumo,
                branch = ramo,
                glyph = def.Icone,
                maxLevel = def.NivelMaximo,
                prices = new Il2CppStructArray<int>(precos),
                requires = new Il2CppReferenceArray<Il2CppSystem.ValueTuple<string, int>>(0),
                requiresAny = false,
                tool = null,
                // Posição provisória: o jogo parece usar layout polar (MoonUpgradeTree.Polar); ajustar ao testar
                position = new Vector2(indice * 1.2f, 0f),
                capstone = false,
                effect = efeito,
            };
        }

        // Nível atual do upgrade. Lê o maior nível entre todas as entradas de treeLevels (time e jogadores),
        // porque ainda não foi confirmado em jogo se a árvore é guardada por jogador ou em MoonWorld.TeamTree.
        public static int Nivel(MoonWorld world, UpgradeDef def)
        {
            var levels = world?.treeLevels;
            if (levels == null) return 0;
            int max = 0;
            foreach (var porJogador in levels)
                if (porJogador.Value != null && porJogador.Value.TryGetValue(def.NodeId, out int nivel) && nivel > max)
                    max = nivel;
            return max;
        }

        private static Il2CppStringArray Append(Il2CppStringArray arr, string item)
        {
            var r = new string[arr.Length + 1];
            for (int i = 0; i < arr.Length; i++) r[i] = arr[i];
            r[arr.Length] = item;
            return new Il2CppStringArray(r);
        }

        private static Il2CppStructArray<Color> Append(Il2CppStructArray<Color> arr, Color item)
        {
            var r = new Color[arr.Length + 1];
            for (int i = 0; i < arr.Length; i++) r[i] = arr[i];
            r[arr.Length] = item;
            return new Il2CppStructArray<Color>(r);
        }
    }
}
