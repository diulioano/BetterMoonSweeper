# Data Model: Mais níveis no Contrato Lunar

## UpgradeDef `ContratoLunar` (existente em `src/Upgrades.cs`)

Só o array `Multiplicadores` muda. Os demais campos ficam iguais à v1.

| Nível | Multiplicador | Texto do efeito | Preço | Novo? |
|---|---|---|---|---|
| 1 | 1,10 | "+10% de dinheiro" | 2.500 | — |
| 2 | 1,25 | "+25% de dinheiro" | 7.500 | — |
| 3 | 1,50 | "+50% de dinheiro" | 22.500 | — |
| 4 | 1,75 | "+75% de dinheiro" | 67.500 | sim |
| 5 | 2,00 | "+100% de dinheiro" | 202.500 | sim |
| 6 | 2,50 | "+150% de dinheiro" | 607.500 | sim |

- `NivelMaximo` = `Multiplicadores.Length` = 6.
- `maxLevel` e `prices` do nó são derivados disso em `FactoryTree.CriarNo`.

## Nível guardado (existente)

- `MoonWorld.treeLevels[jogador]["jonasmod.contract"]`, faixa 0–6. Lido por `FactoryTree.Nivel`.
