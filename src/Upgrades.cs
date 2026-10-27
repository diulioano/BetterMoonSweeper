using System.Globalization;
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
        public string Nome;             // textos visíveis ao jogador: em inglês, no padrão do jogo (spec 005)
        public string Resumo;
        public System.Func<float, string> TextoDoEfeito; // recebe o multiplicador do nível
        public MoonUpgradeTree.Glyph Icone;
        public float[] Multiplicadores; // índice = nível comprado - 1
        public int PrecoBase;           // preço do nível 1; cada nível seguinte custa 3x o anterior

        public int NivelMaximo => Multiplicadores.Length;

        public int PrecoDoNivel(int nivel) => PrecoBase * (int)System.Math.Pow(3, nivel - 1);

        public float Multiplicador(int nivelAtual) => nivelAtual <= 0 ? 1f : Multiplicadores[System.Math.Min(nivelAtual, NivelMaximo) - 1];
    }

    public static class Upgrades
    {
        // Ganho em porcentagem inteira, independente do idioma do Windows: 1.5 → "50"
        private static string Porcento(float m) => ((m - 1f) * 100f).ToString("0", CultureInfo.InvariantCulture);

        // Preços provisórios: ajustar jogando
        public static readonly UpgradeDef CompactadorTurbo = new()
        {
            Id = UpgradeId.CompactadorTurbo,
            NodeId = "bettermoonsweeper.compactor",
            Nome = "Turbo Compactor",
            Resumo = "Overclocked motors: the dust compactor turns dust into sand faster.",
            TextoDoEfeito = m => $"+{Porcento(m)}% compactor speed",
            Icone = MoonUpgradeTree.Glyph.Compactor,
            Multiplicadores = new[] { 1.5f, 2f, 3f },
            PrecoBase = 1000,
        };

        public static readonly UpgradeDef PrensaTurbo = new()
        {
            Id = UpgradeId.PrensaTurbo,
            NodeId = "bettermoonsweeper.stonepress",
            Nome = "Turbo Stone Press",
            Resumo = "Hotter kilns: the stone press bakes blocks faster.",
            TextoDoEfeito = m => $"+{Porcento(m)}% stone press speed",
            Icone = MoonUpgradeTree.Glyph.Motor,
            Multiplicadores = new[] { 1.5f, 2f, 3f },
            PrecoBase = 1500,
        };

        public static readonly UpgradeDef ContratoLunar = new()
        {
            Id = UpgradeId.ContratoLunar,
            NodeId = "bettermoonsweeper.contract",
            Nome = "Lunar Contract",
            Resumo = "A bigger cut of every sale: more money for the whole crew.",
            TextoDoEfeito = m => $"+{Porcento(m)}% payout",
            Icone = MoonUpgradeTree.Glyph.Payout,
            Multiplicadores = new[] { 1.10f, 1.25f, 1.50f, 1.75f, 2.00f, 2.50f },
            PrecoBase = 2500,
        };

        public static readonly UpgradeDef[] Todos = { CompactadorTurbo, PrensaTurbo, ContratoLunar };
    }
}
