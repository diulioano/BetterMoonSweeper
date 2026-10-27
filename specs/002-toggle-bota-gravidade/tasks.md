---

description: "Tarefas: ligar/desligar a bota de gravidade com Ctrl"
---

# Tasks: Ligar/desligar a bota de gravidade com Ctrl

**Input**: Design documents from `/specs/002-toggle-bota-gravidade/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/atalho-teclado.md

**Tests**: sem testes automatizados (inviável contra o jogo IL2CPP); a validação é manual pelo `quickstart.md`.

**Organization**: tarefas agrupadas por user story.

## Format: `[ID] [P?] [Story] Description`

## Phase 1: Setup

- [x] T001 Confirmar que `Unity.InputSystem.dll` existe em `<jogo>\MelonLoader\Il2CppAssemblies` (já referenciados pelo curinga em `BetterMoonSweeper.csproj`); se `Keyboard` não resolver, adicionar a referência explícita em `BetterMoonSweeper.csproj`

---

## Phase 2: Foundational

- [x] T002 Criar `src/GravityBootsHotkey.cs` com a classe estática `GravityBootsHotkey` e o método `public static void Tick()` vazio, envolvido em try/catch que loga a exceção só na primeira vez (flag estática)
- [x] T003 Adicionar `public override void OnUpdate() => GravityBootsHotkey.Tick();` em `src/BetterMoonSweeperMain.cs`

**Checkpoint**: o mod compila e `Tick()` é chamado a cada quadro.

---

## Phase 3: User Story 1 - Alternar a bota com uma tecla (Priority: P1) 🎯 MVP

**Goal**: Ctrl alterna a bota do jogador local.

**Independent Test**: cenários 1 a 4 e 11 do `quickstart.md` (o 11 confirma que a tecla nativa continua funcionando, FR-006).

- [x] T004 [US1] Em `src/GravityBootsHotkey.cs`, sair se `!MoonSession.InGame`; depois detectar o aperto com `leftCtrlKey.wasPressedThisFrame || rightCtrlKey.wasPressedThisFrame`, guardando em cache o `Keyboard` (`MoonInput.PlayKeyboard ?? Keyboard.current`) e os dois `KeyControl`, renovados só quando o cache for nulo ou quando `MoonSession.InGame` passar de falso para verdadeiro (não comparar `PlayKeyboard` por referência a cada quadro); sair cedo se não houver aperto (research R3, plan Performance Goals)
- [x] T005 [US1] Em `src/GravityBootsHotkey.cs`, obter o jogador local com `MoonRunOver.LocalWalker` e sair se for nulo (research R2)
- [x] T006 [US1] Em `src/GravityBootsHotkey.cs`, chamar `bool ok = walker.ToggleGravityBoots()`; se `ok`, logar `Bota de gravidade: ligada/desligada (Ctrl)` com `walker.GravityBootsOn`; se não, logar `ToggleGravityBoots retornou false` (contracts/atalho-teclado.md; valida a hipótese de R1)

**Checkpoint**: com a bota, Ctrl liga e desliga.

---

## Phase 4: User Story 2 - Sem efeito para quem não tem a bota (Priority: P1)

**Goal**: sem a bota, Ctrl não faz nada.

**Independent Test**: cenários 5 e 6 do `quickstart.md`.

- [x] T007 [US2] Em `src/GravityBootsHotkey.cs`, antes de alternar, sair se `!walker.OwnsGravityBoots` (sem log, sem som)

---

## Phase 5: User Story 3 - Não atrapalhar outras ações (Priority: P2)

**Goal**: ignorar Ctrl em chat, menus, veículos, construção e estados ocupados.

**Independent Test**: cenários 7 a 10 do `quickstart.md`.

- [x] T008 [US3] Em `src/GravityBootsHotkey.cs`, sair se `MoonInput.Typing`, `MoonFramePacing.MenuOpen()`, `walker.MenuBlocked` ou `walker.UIBlocked` (research R4; as três últimas são hipóteses, comentar no código; não usar `Cursor.lockState`)
- [x] T009 [US3] Em `src/GravityBootsHotkey.cs`, sair se `walker.Mounted`, `Passenger`, `walker.Riding != null` (`Riding` é um `MoonRobotVacuum`, não `bool`), `Building`, `MoonInput.BuildToolInHand`, `Knocked`, `Climbing`, `Resting`, `Travelling`, `Held` ou `Flying` (spec Edge Cases, research R4)

---

## Phase 6: User Story 4 - Os outros jogadores veem o estado certo (Priority: P2)

**Goal**: a troca feita com Ctrl aparece para os outros jogadores.

**Independent Test**: cenário 12 do `quickstart.md`.

Não há código próprio: a sincronização vem de `ToggleGravityBoots()` (research R1). A validação e o
fallback condicional estão em T014.

---

## Phase 7: Polish

- [x] T010 [P] Em `src/GravityBootsHotkey.cs`, dentro de `#if DIAGNOSTICO` (em `BetterMoonSweeper.csproj`, adicionar `<DefineConstants Condition="'$(Diagnostico)' == 'true'">$(DefineConstants);DIAGNOSTICO</DefineConstants>`, para `dotnet build -c Release -p:Diagnostico=true` ligar o diagnóstico; o build Release normal roda por último, para a DLL sem diagnóstico ser a que fica em `<jogo>\Mods`), na primeira partida percorrer por reflexão as propriedades estáticas de `MoonInput` do tipo `MoonInput.Binding` e logar as que têm `Key.LeftCtrl` ou `Key.RightCtrl` em `keys` (research R6)
- [x] T011 [P] Atualizar `README.md`: estado da spec 002 na tabela de especificações e o atalho Ctrl numa seção "Controles do mod"
- [x] T012 **Obrigatório**: subir `BetterMoonSweeperMain.Version` em `src/BetterMoonSweeperMain.cs` para uma versão que nenhum build distribuído já usou, conforme research R7 (0.2.0 se for a primeira release depois da 0.1.0, ou se 002 e 003 saírem juntas)
- [x] T013 Rodar `dotnet build -c Release` sem erros
- [ ] T014 **Bloqueado (MelonLoader)**: executar todos os cenários de `specs/002-toggle-bota-gravidade/quickstart.md` em jogo, incluindo co-op (cenário 12) e o diagnóstico de T010. Se o cenário 12 não sincronizar ou o 1 ficar sem som, aplicar o fallback de R1 em `src/GravityBootsHotkey.cs` e registrar o resultado em `research.md`. Se T010 achar Ctrl em alguma binding, revisar o edge case da spec

---

## Dependencies & Execution Order

- T001 → T002 → T003 → US1 (T004–T006) → US2 (T007) → US3 (T008–T009) → Polish (T010–T013) → T014
- Todas as tarefas de US1 a US3 mexem no mesmo arquivo, então são sequenciais (sem [P]).
- T010 e T011 podem rodar em paralelo depois de T006.
- T014 depende do desbloqueio externo do MelonLoader.

## Parallel Example

```text
Depois de T006:
Task: "T010 Diagnóstico de bindings com Ctrl em src/GravityBootsHotkey.cs"
Task: "T011 Atualizar README.md"
```

## Implementation Strategy

1. Setup + Foundational (T001–T003): compila.
2. US1 + US2 (T004–T007): MVP; já é o pedido do usuário.
3. US3 (T008–T009): evita trocas acidentais.
4. Polish (T010–T013) e, quando o MelonLoader voltar, T014: validação em jogo e co-op.
