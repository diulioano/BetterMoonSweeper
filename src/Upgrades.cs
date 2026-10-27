using MoonCleaners;

namespace BetterMoonSweeper
{
    public enum UpgradeId
    {
        CompactadorTurbo,
        PrensaTurbo,
        ContratoLunar,
    }

    public sealed class UpgradeDef
    {
        public UpgradeId Id;
        public string NodeId;           // id do nó na MoonUpgradeTree; também é a chave em MoonWorld.treeLevels
        public string Nome;
        public string Resumo;
        public MoonUpgradeTree.Glyph Icone;
        public float[] Multiplicadores; // índice = nível comprado - 1
        public int PrecoBase;           // preço do nível 1; cada nível seguinte custa 3x o anterior

        public int NivelMaximo => Multiplicadores.Length;

        public int PrecoDoNivel(int nivel) => PrecoBase * (int)System.Math.Pow(3, nivel - 1);

        public float Multiplicador(int nivelAtual) => nivelAtual <= 0 ? 1f : Multiplicadores[System.Math.Min(nivelAtual, NivelMaximo) - 1];
    }

    public static class Upgrades
    {
        // Preços provisórios: ajustar jogando
        public static readonly UpgradeDef CompactadorTurbo = new()
        {
            Id = UpgradeId.CompactadorTurbo,
            NodeId = "jonasmod.compactor",
            Nome = "Compactador Turbo",
            Resumo = "O compactador transforma poeira em areia mais rápido.",
            Icone = MoonUpgradeTree.Glyph.Compactor,
            Multiplicadores = new[] { 1.5f, 2f, 3f },
            PrecoBase = 1000,
        };

        public static readonly UpgradeDef PrensaTurbo = new()
        {
            Id = UpgradeId.PrensaTurbo,
            NodeId = "jonasmod.stonepress",
            Nome = "Prensa Turbo",
            Resumo = "A prensa de pedra fabrica blocos mais rápido.",
            Icone = MoonUpgradeTree.Glyph.Motor,
            Multiplicadores = new[] { 1.5f, 2f, 3f },
            PrecoBase = 1500,
        };

        public static readonly UpgradeDef ContratoLunar = new()
        {
            Id = UpgradeId.ContratoLunar,
            NodeId = "jonasmod.contract",
            Nome = "Contrato Lunar",
            Resumo = "Aumenta todo o dinheiro recebido pelo time.",
            Icone = MoonUpgradeTree.Glyph.Payout,
            Multiplicadores = new[] { 1.10f, 1.25f, 1.50f, 1.75f, 2.00f, 2.50f },
            PrecoBase = 2500,
        };

        public static readonly UpgradeDef[] Todos = { CompactadorTurbo, PrensaTurbo, ContratoLunar };
    }
}
