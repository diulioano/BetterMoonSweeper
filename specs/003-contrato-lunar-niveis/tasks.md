---

description: "Tarefas: mais níveis no Contrato Lunar"
---

# Tasks: Mais níveis no Contrato Lunar

**Input**: Design documents from `/specs/003-contrato-lunar-niveis/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/no-contrato-lunar.md

**Tests**: sem testes automatizados; a validação é manual pelo `quickstart.md`.

**Organization**: tarefas agrupadas por user story.

## Format: `[ID] [P?] [Story] Description`

## Phase 1: Setup

Nenhuma tarefa: o projeto e as referências já existem.

## Phase 2: Foundational

Nenhuma tarefa: `UpgradeDef`, `FactoryTree.CriarNo` e `PayoutMultiplierPatch` já derivam tudo de `Multiplicadores` (research R1).

---

## Phase 3: User Story 1 - Comprar níveis além do 3 (Priority: P1) 🎯 MVP

**Goal**: o Contrato Lunar tem 6 níveis.

**Independent Test**: cenários 1 a 5 do `quickstart.md`.

- [x] T001 [US1] Em `src/Upgrades.cs`, mudar `ContratoLunar.Multiplicadores` para `new[] { 1.10f, 1.25f, 1.50f, 1.75f, 2.00f, 2.50f }`, sem alterar `PrecoBase = 2500` nem os outros campos (data-model.md)
- [x] T002 [US1] Em `src/Upgrades.cs`, conferir que `PrecoDoNivel(6)` = 607500 cabe em `int` e que o texto `+{(m - 1f) * 100f:0}% de dinheiro` em `FactoryTree.CriarNo` (`src/FactoryTree.cs`) gera "+75%", "+100%" e "+150%" para os níveis 4 a 6; sem mudança de código esperada

**Checkpoint**: compila; o nó tem 6 níveis.

---

## Phase 4: User Story 2 - Partidas em andamento continuam valendo (Priority: P1)

**Goal**: níveis comprados na 0.1.0 continuam valendo.

**Independent Test**: cenário 6 do `quickstart.md`.

- [x] T003 [US2] Conferir em `src/Upgrades.cs` que `NodeId` do Contrato Lunar continua `jonasmod.contract` e que os três primeiros multiplicadores e o `PrecoBase` são os da v1 (research R4); sem mudança de código esperada

---

## Phase 5: Polish

- [x] T004 **Obrigatório**: subir `BetterMoonSweeperMain.Version` em `src/BetterMoonSweeperMain.cs` para uma versão que nenhum build distribuído usou (0.2.0 se for a primeira release depois da 0.1.0, ou se 002 e 003 saírem juntas), conforme research R6
- [x] T005 [P] Atualizar `README.md`: linha do Contrato Lunar na tabela "Fase 1: design acordado" (+10% / +25% / +50% / +75% / +100% / +150%) e estado da spec 003 na tabela de especificações como "Implementada, não testada" (constituição VI)
- [x] T006 [P] Atualizar `specs/001-fabrica-v1/spec.md`, seção "Status por requisito": linha de FR-003/FR-006 indicando que o código já tem os 6 níveis da spec 003 (o texto normativo de FR-003, FR-006 e US3 da 001 já foi emendado durante o planejamento)
- [x] T007 Rodar `dotnet build -c Release` sem erros
- [ ] T008 **Bloqueado (MelonLoader)**: executar todos os cenários de `specs/003-contrato-lunar-niveis/quickstart.md`, incluindo saves da 0.1.0 (cenário 6) e co-op (cenário 7). Se o nível 4 não for comprável, aplicar o fallback A (research R2, Postfix em `TreeMaxLevel` em `src/Patches.cs`) e registrar em `research.md`. O fallback B (nó "Contrato Lunar II") só depois de A falhar e de emendar a spec FR-004 via `/speckit-clarify`. Marcadores apertados são cosméticos e aceitos (R3)

---

## Dependencies & Execution Order

- T001 → T002 → T003 → T004 → T007 → T008.
- T005 e T006 podem rodar em paralelo depois de T001.
- T008 depende do desbloqueio externo do MelonLoader.

## Parallel Example

```text
Depois de T001:
Task: "T005 Atualizar README.md"
Task: "T006 Atualizar specs/001-fabrica-v1/spec.md"
```

## Implementation Strategy

1. T001–T003: a feature inteira (uma linha de código mais conferências).
2. T004–T007: versão, documentação e build.
3. T008 quando o MelonLoader voltar a rodar mods.
