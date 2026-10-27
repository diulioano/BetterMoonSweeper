---

description: "Tarefas: textos do mod em inglês"
---

# Tasks: Textos do mod em inglês, no padrão do jogo

**Input**: `/specs/005-textos-ingles/` (spec.md, plan.md, quickstart.md)

## Phase 1: Setup

- [x] T001 Emendar `.specify/memory/constitution.md` (1.1.0 → 1.2.0, MINOR): texto visível ao jogador em inglês, no padrão do jogo; documentação, logs e comunicação em português; Sync Impact Report

## Phase 2: User Story 1 - A aba do mod parece parte do jogo (Priority: P1) 🎯 MVP

**Independent Test**: `quickstart.md` cenários 1 a 3, num Windows em português.

- [x] T002 [US1] Em `src/Upgrades.cs`: adicionar `using System.Globalization;` e o campo `public System.Func<float, string> TextoDoEfeito;` em `UpgradeDef`; trocar `Nome`/`Resumo` dos 3 upgrades pelos textos da tabela do plan.md; definir `TextoDoEfeito` com **uma única forma** de formatação, por um helper `Porcento(float m) => ((m - 1f) * 100f).ToString("0", CultureInfo.InvariantCulture)`: Compactador `m => $"+{Porcento(m)}% compactor speed"`, Prensa `"... stone press speed"`, Contrato `"... payout"` (a interpolação recebe só a string pronta, sem formatar número)
- [x] T003 [US1] Em `src/FactoryTree.cs`: `NomeDoRamo = "Factory"`, `SubtituloDoRamo = "Faster machines, bigger payouts"`, e o `if` de `CriarNo` vira `def.TextoDoEfeito(def.Multiplicador(nivel))`; em `src/BetterMoonSweeperMain.cs`, `Version = "0.3.1"`
- [ ] T004 [US1] Rodar `dotnet build -c Release -p:Diagnostico=true` e por último `dotnet build -c Release` (jogo fechado), sem erros
- [ ] T005 [US1] Executar `specs/005-textos-ingles/quickstart.md` (todos os cenários, incluindo `effect(0)`, pontuação e save/reload) numa sessão descartável. Pode rodar junto com a T015 da spec 004

- [x] T008 [US1] Em `src/Upgrades.cs`, trocar os `NodeId` de `jonasmod.*` para `bettermoonsweeper.*` (FR-005)

## Phase 3: Polish

- [x] T006 [P] Atualizar `README.md` (tabela de upgrades, nome da aba, versão 0.3.1) e `CLAUDE.md` (regra de idioma)
- [x] T007 [P] Nas specs anteriores, sem reescrever o histórico: trocar "aba Fábrica" por "aba Factory" nos quickstarts das specs 001 a 004 e na T015 da spec 004; no quickstart da 003, os textos de efeito ("+75% de dinheiro" → "+75% payout"); e acrescentar uma nota "nomes visíveis substituídos pela spec 005" na spec 001 (FR-001), na spec 003 (US1-5, `data-model.md`, `contracts/no-contrato-lunar.md`) e na spec 004 (US1)

## Dependencies

T001 → T002 → T003 → T004 → T005. A T005 depende de o mod já rodar em jogo (T015 da spec 004) e pode ser feita na mesma sessão de teste. T006 e T007 a qualquer momento depois de T003.
