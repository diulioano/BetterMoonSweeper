# Data Model: Mais níveis no Contrato Lunar

## UpgradeDef `ContratoLunar` (existente em `src/Upgrades.cs`)

Só o array `Multiplicadores` muda. Os demais campos ficam iguais à v1.

| Nível | Multiplicador | Texto do efeito | Preço | Novo? |
|---|---|---|---|---|
| 1 | 1,10 | "+10% payout" | 2.500 | — |
| 2 | 1,25 | "+25% payout" | 7.500 | — |
| 3 | 1,50 | "+50% payout" | 22.500 | — |
| 4 | 1,75 | "+75% payout" | 67.500 | sim |
| 5 | 2,00 | "+100% payout" | 202.500 | sim |
| 6 | 2,50 | "+150% payout" | 607.500 | sim |

- `NivelMaximo` = `Multiplicadores.Length` = 6.
- `maxLevel` e `prices` do nó são derivados disso em `FactoryTree.CriarNo`.

## Nível guardado (existente)

- `MoonWorld.treeLevels[jogador]["bettermoonsweeper.contract"]` (era `jonasmod.contract` até a spec 005), faixa 0–6. Lido por `FactoryTree.Nivel`.
