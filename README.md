# BetterMoonSweeper — Moon Sweepers

Mod de upgrades de fábrica e qualidade de vida para o Moon Sweepers (Unity 6000.6, IL2CPP), plugin do
BepInEx 6. Versão atual: **0.3.0**. Até a 0.1.0 o mod se chamava JonasMod; até a 0.2.0 usava MelonLoader.

## Instalar (jogadores)

Todos do grupo precisam da mesma instalação e da mesma versão do mod.

1. **Instalação antiga (só se você tinha a 0.1.0/0.2.0):** apague da pasta do jogo `version.dll`,
   `MelonLoader\`, `Mods\` (com a `JonasMod.dll`), `Plugins\`, `UserData\` e `UserLibs\`.
2. Baixe o **BepInEx 6.0.0-be.791**, versão `Unity.IL2CPP` win-x64, no site oficial
   ([builds.bepinex.dev](https://builds.bepinex.dev/projects/bepinex_be), arquivo
   `BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.791+b782da4.zip`) e extraia na pasta do jogo, ao lado do
   `MoonSweepers.exe`.
3. Baixe o **Moon Sweepers BepInEx Patcher V1.0.0** no Nexus
   ([mod 9](https://www.nexusmods.com/moonsweepers/mods/9)) e extraia na pasta do jogo. Deve ficar
   `BepInEx\patchers\MoonSweepers.ModEngine\MoonSweepers.ModEngine.dll`
   (SHA-256 `b91bb4d4c12870abe79039a04173bbd5077322a8613bf90159a200a8370b1f94`).
4. Abra o jogo uma vez e feche. O primeiro início demora um pouco mais.
5. Copie `BetterMoonSweeper.dll` para `BepInEx\plugins`.
6. Abra o jogo. Em `BepInEx\LogOutput.log` deve aparecer `BetterMoonSweeper 0.3.0 carregado.` e,
   no fim, `Chainloader startup complete`. As mensagens `Unsupported metadata version found! ...
   got 108` e `Class::Init signatures have been exhausted` são esperadas.

## Desinstalar

Apague da pasta do jogo: `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`, `changelog.txt`,
`dotnet\` e `BepInEx\`. O jogo volta ao original.

**Saves:** os níveis dos upgrades do mod ainda ficam no save do jogo (o save separado não foi feito).
Faça backup de `%USERPROFILE%\AppData\LocalLow\DragDropGames\MoonSweepers` antes de jogar com o mod.

## Status

- [x] BepInEx 6.0.0-be.791 + MoonSweepers BepInEx Patcher instalados; o mod carrega (0.3.0, 2026-10-27)
- [x] .NET 8 SDK instalado; o mod compila
- [~] Fase 1, atalho Ctrl e Contrato Lunar com 6 níveis: carregam, **testes em jogo em andamento**
- Histórico: com o MelonLoader 0.7.3 o mod carregava mas não executava (`No Support Module Loaded!`,
  Il2CppInterop sem suporte à Unity 6000.6). Por isso a migração para BepInEx (spec 004).
- [ ] Fase 2: JONAS 2.0 (o Jonas se chama `Joonas` no código: `MoonJoonasRig`, `MoonJoonasMotion`, `MoonJoonasGauge`)

## Especificações (Spec Kit)

O projeto usa o [Spec Kit](https://github.com/github/spec-kit) (`specify`). Regras do projeto em
[.specify/memory/constitution.md](.specify/memory/constitution.md); cada feature tem spec, plano e
tarefas em `specs/`:

| Spec | Feature | Estado |
|---|---|---|
| [001](specs/001-fabrica-v1/spec.md) | Fábrica v1 (linha de base deste README) | Parcial: falta bloqueio no lobby e save separado; nada testado em jogo |
| [002](specs/002-toggle-bota-gravidade/spec.md) | Ctrl liga/desliga a bota de gravidade | Implementada, não testada |
| [003](specs/003-contrato-lunar-niveis/spec.md) | Contrato Lunar com 6 níveis (+75% / +100% / +150% nos novos) | Implementada, não testada |
| [004](specs/004-migracao-bepinex/spec.md) | Migração para BepInEx 6 | Implementada; mod carrega; testes em jogo em andamento |

Fluxo para features novas: `/speckit-specify` → `/speckit-plan` → `/speckit-tasks` →
`/speckit-analyze` → `/speckit-implement`.

## Fase 1: design acordado

| Decisão | Escolha |
|---|---|
| Multiplayer | Todos instalam o mod; quem não tiver ou estiver em versão diferente é bloqueado no lobby |
| Upgrades | Compactador Turbo (1,5× / 2× / 3×), Prensa Turbo (stone fabric, mesmos níveis), Contrato Lunar (+10% / +25% / +50% / +75% / +100% / +150% de dinheiro, 6 níveis desde a 0.2.0) |
| Preço | Cada nível custa ~3× o anterior (valores provisórios em `src/Upgrades.cs`) |
| Pagamento | Qualquer jogador compra com o dinheiro do time; o host confirma |
| Save | Arquivo separado do mod no PC do host; o save original não é alterado |
| Interface | Aba nova "Fábrica" na árvore de upgrades; se não der, os nós vão para o ramo Economy |

## Controles do mod

| Tecla | O que faz |
|---|---|
| Ctrl (esquerdo ou direito) | Liga/desliga a bota de gravidade, se o jogador já a comprou. Ignorado no chat, em menus, em veículos/robô, no modo de construção, caído, na escada, descansando, em viagem ou sendo carregado. Sem suporte a controle (gamepad) |

## Como o jogo funciona por dentro

Tudo fica no namespace `MoonCleaners` (nas DLLs geradas em `BepInEx\interop`). A lista completa de
classes está em [docs/assembly-csharp.txt](docs/assembly-csharp.txt), gerada por `tools/ClassDump`.

### Máquinas de produção

| No jogo | Classe visual | Spec (estática em `MoonWorld`) |
|---|---|---|
| Compactador (poeira → areia) | `MoonPressureChamberView` | `ChamberSpec`, `PressureChamberKind` |
| Stone fabric (poeira + areia → bloco de pedra) | `MoonStonePressView` | `PressSpec`, `StonePressKind` |
| Painel solar (energia) | `MoonSolarPanelView` | — |

- As classes `*View` só animam. A lógica fica em `MoonWorld`: `StepProduction(dt)`, `CanStartCycle`,
  `TryStartCycle`, `FinishCycle`, `EmitProduct`. O estado de cada máquina é um `MoonProductionState`
  (`Progress`, `Running`, `Cycles`, `stores`), guardado em `MoonWorld.production`.
- `ProcessorSpec` tem `cycleSeconds`, `runningKw`, `slotCapacity` e `output`. **O mod acelera as máquinas
  dividindo o `cycleSeconds`.**
- Em co-op, `WriteCrewProduction` e `ApplyCrewProduction` sincronizam a produção; `hostCycles` sugere
  que o host é a autoridade.

### Árvore de upgrades

- `MoonUpgradeTree` (estática): `Nodes`, `byId`, `BranchNames`, `BranchColors`, `BranchTaglines`, `Build()`.
- Ramos (`MoonUpgradeTree.Branch`): Core, Containers, Cleaning, Suit, Logistics, Economy.
- `MoonUpgradeTree.Node`: `id`, `title`, `summary`, `branch`, `glyph`, `maxLevel`, `prices` (int por nível),
  `requires`, `position`, `effect` (`Func<int, string>`, texto do efeito por nível).
- Ícones (`Glyph`) úteis: `Compactor`, `Motor`, `Payout`, `Exchange`, `Turbo`.
- Compra: `MoonWorld.TryGetTreePrice`, `TryPurchaseTreeNode(player, id, expectedLevel, ref message)`,
  `TreeLevel(player, id)`. Os níveis ficam em `MoonWorld.treeLevels` (`player → id → nível`), e existe
  uma chave de time `MoonWorld.TeamTree`.
- Compras em co-op passam por `CrewPurchase(player, PurchaseKind.TreeNode, id, level)`.
- Save: `MoonUpgradeTreeSave.Write/Read`, `SnapshotUpgradeTree/RestoreUpgradeTree`.
- Tela: `MoonUpgradeTreeUI` (`Build`, `ShowBranch`, `StepBranch`, `tabs`, `InstallPicked`).

### Dinheiro

- `MoonWorld.wallets` (`TeamWallet`, `Balance`), `WalletDollars(player)`, `SetTeamDollars`, `MoneyChanged`.
- Multiplicadores que já existem: `PayoutMultiplier(player)` e `PriceMultiplier(player)`, ambos `Il2CppSystem.Decimal`.
  **O Contrato Lunar multiplica o resultado de `PayoutMultiplier`.**
- Menu de dev: `DevAddMoney(playerId, dollars)`, `DevSetCarriedDust`, `DevResetDust`.

## Estrutura do código

| Arquivo | O que faz |
|---|---|
| `src/BetterMoonSweeperMain.cs` | Ponto de entrada (plugin do BepInEx); aplica os patches Harmony |
| `src/Upgrades.cs` | Definição dos 3 upgrades (ids, preços, multiplicadores). Os ids `jonasmod.*` mantêm o nome antigo de propósito |
| `src/GravityBootsHotkey.cs` | Atalho Ctrl da bota de gravidade |
| `src/FactoryTree.cs` | Injeta o ramo "Fábrica" e os nós na `MoonUpgradeTree` |
| `src/Patches.cs` | Patches Harmony: velocidade das máquinas, multiplicador de dinheiro e chamada por quadro do atalho Ctrl (`MoonWorld.Update`) |

### Ainda não implementado

- Bloquear no lobby quem não tem o mod ou está em outra versão.
- Tirar os nós do mod do save original e guardar num arquivo separado.

## Compilar

Pré-requisitos: .NET SDK 6 ou mais novo, e o jogo aberto pelo menos uma vez com o BepInEx e o patcher
(isso gera `BepInEx\interop`).

```
dotnet build -c Release
```

O build copia `BetterMoonSweeper.dll` para `<pasta do jogo>\BepInEx\plugins` (com o jogo fechado; a
DLL fica travada enquanto ele roda). Para usar outra pasta do jogo:
`dotnet build -p:GameDir="D:\caminho\Moon Sweepers"`.

Build de diagnóstico, com logs extras para investigar o jogo:
`dotnet build -c Release -p:Diagnostico=true`. Depois rode o build normal de novo, para a DLL sem
diagnóstico ficar em `BepInEx\plugins`.

Para regenerar a lista de classes depois de uma atualização do jogo:

```
dotnet run --project tools/ClassDump -c Release -- "<jogo>\BepInEx\interop\Assembly-CSharp.dll" docs/assembly-csharp.txt
```

## Backups locais

Em `Documents\MoonSweepers-backup-2026-10-27`: estado do jogo antes de qualquer loader, `melonloader\`
(instalação antiga do MelonLoader), `Mods-antes-do-rename\` (`JonasMod.dll`) e `saves-antes-bepinex\`.
