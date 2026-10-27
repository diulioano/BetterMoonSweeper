---

description: "Tarefas: migração do mod para BepInEx 6"
---

# Tasks: Migração do mod para BepInEx 6

**Input**: Design documents from `/specs/004-migracao-bepinex/`

**Prerequisites**: plan.md, spec.md, research.md, quickstart.md

**Tests**: sem testes automatizados; validação manual pelo `quickstart.md`.

## Format: `[ID] [P?] [Story] Description`

## Phase 1: Setup

- [x] T001 Mover o MelonLoader (`version.dll`, `MelonLoader\`, `Mods\`, `Plugins\`, `UserData\`, `UserLibs\`) da pasta do jogo para `Documents\MoonSweepers-backup-2026-10-27\melonloader`
- [x] T002 Baixar `BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.791+b782da4.zip` de builds.bepinex.dev (SHA-256 `bb1c510909b69522733791f3a0817a487379a52da6468a492f1c9c19b3ec6f9a`) e extrair na pasta do jogo
- [x] T003 Emendar `.specify/memory/constitution.md` para 1.1.0 **antes de qualquer build**: princípio II com `BepInEx\plugins` no lugar de `Mods`; princípio VI sem a referência ao bloqueio do MelonLoader; Restrições Técnicas com BepInEx 6 be.791 + patcher e o namespace real dos tipos do jogo (preencher depois de T007); Sync Impact Report
- [x] T004 Instalar o patcher baixado pelo usuário do Nexus (mod 9) em `BepInEx\patchers\MoonSweepers.ModEngine\` e registrar nome, versão e SHA-256 em `specs/004-migracao-bepinex/research.md` (R1 e R6)
- [x] T005 Abrir o jogo uma vez **no menu principal, sem carregar sessão**, e confirmar no `BepInEx\LogOutput.log` a linha `Chainloader startup complete` e as DLLs em `BepInEx\interop` (quickstart cenário 1)

---

## Phase 2: Foundational

- [x] T006 Fazer backup de `%USERPROFILE%\AppData\LocalLow\DragDropGames\MoonSweepers\` (inclui `MoonSessions` e `Backups`) em `Documents\MoonSweepers-backup-2026-10-27\saves-antes-bepinex` (constituição III; ver Complexity Tracking do plano)
- [x] T007 Regenerar `docs/assembly-csharp.txt` com `tools/ClassDump` sobre `BepInEx\interop\Assembly-CSharp.dll` e registrar em research R4 o namespace real dos tipos do jogo e de `Keyboard`/`Key`
- [x] T008 Em `BetterMoonSweeper.csproj`, trocar as referências do MelonLoader por `BepInEx\core\{BepInEx.Core,BepInEx.Unity.IL2CPP,0Harmony,Il2CppInterop.Runtime,Il2CppInterop.Common}.dll` e `BepInEx\interop\*.dll` (`Private=false`), e o deploy para `$(GameDir)\BepInEx\plugins`, condicionado a `Exists('$(GameDir)\BepInEx')` (research R5)
- [x] T009 Ajustar os `using` de `src/*.cs` para os namespaces encontrados em T007

---

## Phase 3: User Story 1 - O mod roda no jogo (Priority: P1) 🎯 MVP

**Goal**: o mod carrega no BepInEx e as features das specs 001 a 003 funcionam.

**Independent Test**: quickstart cenários 2 a 6.

- [x] T010 [US1] Em `src/BetterMoonSweeperMain.cs`, trocar `MelonMod` por `BasePlugin` com `[BepInPlugin("com.diuli.bettermoonsweeper", "BetterMoonSweeper", Version)]`; em `Load()`, guardar `Log` num `public static ManualLogSource Log`, logar `BetterMoonSweeper {Version} carregado` e chamar `new Harmony("com.diuli.bettermoonsweeper").PatchAll(typeof(BetterMoonSweeperMain).Assembly)`; remover `OnUpdate`, `MelonInfo`/`MelonGame`; trocar o comentário "aplicados automaticamente pelo MelonLoader" e manter o TODO do lobby; subir `Version` para `0.3.0` (research R2, R7)
- [x] T011 [US1] Em `src/Patches.cs`, adicionar `[HarmonyPatch(typeof(MoonWorld), nameof(MoonWorld.Update))]` com Postfix chamando `GravityBootsHotkey.Tick()` (research R3)
- [x] T012 [US1] Em `src/GravityBootsHotkey.cs` (inclusive as linhas dentro de `#if DIAGNOSTICO`, que usam `var log = Melon<...>.Logger`) e `src/FactoryTree.cs`, trocar `Melon<BetterMoonSweeperMain>.Logger` (`Msg`/`Error`) por `BetterMoonSweeperMain.Log` (`LogInfo`/`LogError`) e remover `using MelonLoader`
- [x] T013 [US1] Em `src/FactoryTree.cs`, tornar `Injetar()` à prova de falha parcial (research R3, H5): montar todos os nós (incluindo a conversão de `effect` para `Il2CppSystem.Func`) **antes** de alterar `BranchNames`/`BranchTaglines`/`BranchColors`; envolver o corpo em try/catch que loga só a primeira exceção e marca a injeção como desistida, para os Prefix de `StepProduction` e `Build` não tentarem de novo a cada quadro
- [x] T014 [US1] Rodar `dotnet build -c Release -p:Diagnostico=true` e **por último** `dotnet build -c Release`, os dois sem erros (quickstart cenário 2)
- [ ] T015 [US1] Abrir o jogo, **criar uma sessão nova descartável** (não usar os saves de verdade) e executar os cenários 3 a 6 do quickstart, conferindo que existe **uma única** aba "Factory" (nome da spec 005); depois, os cenários solo dos quickstarts das specs 002 e 003

**Checkpoint**: mod rodando e testado em jogo, solo.

---

## Phase 4: User Story 2 - Amigos instalam sozinhos (Priority: P1)

**Goal**: guia de instalação e desinstalação no README, validado.

**Independent Test**: quickstart cenários 7 e 8.

- [x] T016 [US2] Em `README.md`, escrever "Instalar (jogadores)" e "Desinstalar" conforme research R6, com nome, versão e SHA-256 do patcher (de T004), a remoção da instalação antiga (MelonLoader e `JonasMod.dll`), e substituir a seção "Desinstalar o MelonLoader"
- [ ] T017 [US2] Um amigo instala seguindo só o README (SC-003); depois, executar os cenários co-op das specs 002 (cenário 12) e 003 (cenário 7) e o SC-004 da spec 001
- [ ] T018 [US2] Executar o cenário 8 do quickstart (desinstalar e abrir o jogo sem mods) e restaurar a instalação (SC-004)

---

## Phase 5: User Story 3 - Desenvolvimento continua funcionando (Priority: P2)

**Goal**: build e documentação com BepInEx.

**Independent Test**: quickstart cenário 2.

- [x] T019 [P] [US3] Em `README.md` e `CLAUDE.md`, trocar **todas** as menções a MelonLoader, `Mods`, `Il2CppMoonCleaners` e `Latest.log` pelo equivalente no BepInEx (`BepInEx\plugins`, namespace de T007, `BepInEx\LogOutput.log`, `BepInEx\interop`, comando do ClassDump), deixando o bloqueio do MelonLoader só como histórico

---

## Phase 6: Polish

- [x] T020 [P] Nos quickstarts das specs 002 e 003, trocar o pré-requisito "MelonLoader executando mods" e o caminho do log por BepInEx
- [ ] T021 [P] Conferir manualmente o SC-006 da spec 001: carregar a sessão descartável de T015 sem o mod e ver se abre (registrar o resultado em `specs/001-fabrica-v1/spec.md`)

---

## Dependencies & Execution Order

- T003 antes de T008 e T014 (constituição II).
- T004 depende do usuário baixar o patcher. T005 depende de T004. T006 antes de T015.
- T007 depende de T005 (as DLLs de interop só existem depois do primeiro início).
- T008 → T009 → T010–T013 → T014 → T015.
- T017 depende de T016 (o amigo precisa do guia).
- T019 e T020 podem ser feitas a qualquer momento depois de T007.

## Implementation Strategy

1. Setup (T001–T005): loader funcionando e constituição emendada.
2. Foundational + US1 (T006–T015): mod rodando e testado solo, com saves protegidos (MVP).
3. US2: guia, teste com um amigo, desinstalação.
4. US3 e Polish: documentação.
