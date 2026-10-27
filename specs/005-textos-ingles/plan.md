# Implementation Plan: Textos do mod em inglês, no padrão do jogo

**Branch**: `005-textos-ingles` | **Date**: 2026-10-27 | **Spec**: [spec.md](spec.md)

## Summary

Traduzir os 8 textos visíveis do mod (aba, subtítulo, 3 nomes, 3 resumos) e os 2 formatos de efeito
para o inglês no estilo do jogo, e formatar os números com `CultureInfo.InvariantCulture`.
Os textos de efeito passam para `UpgradeDef.TextoDoEfeito`, para cada upgrade ter o seu (princípio IV:
tudo do upgrade em `Upgrades.cs`), e `FactoryTree.CriarNo` deixa de ter o `if` por upgrade.

## Technical Context

**Language/Version**: C# net6.0 | **Primary Dependencies**: BepInEx 6 be.791 | **Testing**: manual em jogo
**Project Type**: mod | **Scale/Scope**: `src/Upgrades.cs`, `src/FactoryTree.cs`, constituição, README

## Research

- Estilo lido de `global-metadata.dat` (strings literais do jogo), ver Assumptions da spec.
- O jogo já tem um nó "Market Contracts" ("Better contracts for lunar dust: more money for every
  kilogram launched", "+10% launch payout"). O Contrato Lunar do mod multiplica **todo** o pagamento
  (`PayoutMultiplier`), não só o lançamento, então o efeito diz "payout", sem "launch".
- `$"{m:0.#}"` usa a cultura atual: num Windows pt-BR gera "1,5". Usar `ToString("0.#", CultureInfo.InvariantCulture)`.
- Velocidade em porcentagem, como o jogo ("+8% speed"): multiplicador 1.5 → "+50%", 2 → "+100%", 3 → "+200%".

## Textos

| Onde | Antes | Depois |
|---|---|---|
| Aba | Fábrica | Factory |
| Subtítulo | Máquinas mais rápidas, mais lucro. | Faster machines, bigger payouts |
| Nó | Compactador Turbo | Turbo Compactor |
| Resumo | O compactador transforma poeira em areia mais rápido. | Overclocked motors: the dust compactor turns dust into sand faster. |
| Efeito | 1,5x mais rápido | +50% compactor speed |
| Nó | Prensa Turbo | Turbo Stone Press |
| Resumo | A prensa de pedra fabrica blocos mais rápido. | Hotter kilns: the stone press bakes blocks faster. |
| Efeito | 1,5x mais rápido | +50% stone press speed |
| Nó | Contrato Lunar | Lunar Contract |
| Resumo | Aumenta todo o dinheiro recebido pelo time. | A bigger cut of every sale: more money for the whole crew. |
| Efeito | +10% de dinheiro | +10% payout |

## Versão

- Sobe para **0.3.1**: não muda lógica sincronizada, mas muda o que o jogador vê; a mesma versão para
  todos evita amigos com textos diferentes (constituição I).

## Constitution Check

| Princípio | Situação |
|---|---|
| I, II, III, V, VI | ✅ Sem mudança de comportamento, ids ou deploy |
| IV | ✅ Textos dos efeitos passam a ficar em `Upgrades.cs` |
| Restrições Técnicas | ⚠️ Hoje exige texto para o jogador em português: emendar (MINOR → 1.2.0, muda o conteúdo da regra) para "inglês, no padrão do jogo" |

## Complexity Tracking

Nenhuma violação: a emenda da constituição vem antes do código (T001).
