# JonasMod — Moon Sweepers

Mod de upgrades de fábrica para o Moon Sweepers (Unity 6000.6, IL2CPP), feito com MelonLoader v0.7.3.

## Status

- [x] MelonLoader v0.7.3 instalado na pasta do jogo
- [x] Geração das DLLs do jogo (com Cpp2IL da branch `development`, que suporta metadata 108)
- [x] .NET 8 SDK instalado; o mod compila
- [ ] **Bloqueado:** o MelonLoader não executa código de mod na Unity 6.6 (`No Support Module Loaded!`).
      Aguardando LavaGang/MelonLoader#1180 (Il2CppInterop v2). Ver issue LavaGang/MelonLoader#1159.
- [~] Fase 1: patches escritos e compilando, **nunca testados em jogo**
- [ ] Fase 2: JONAS 2.0 (o Jonas se chama `Joonas` no código: `MoonJoonasRig`, `MoonJoonasMotion`, `MoonJoonasGauge`)

## Fase 1: design acordado

| Decisão | Escolha |
|---|---|
| Multiplayer | Todos instalam o mod; quem não tiver ou estiver em versão diferente é bloqueado no lobby |
| Upgrades | Compactador Turbo (1,5× / 2× / 3×), Prensa Turbo (stone fabric, mesmos níveis), Contrato Lunar (+10% / +25% / +50% de dinheiro) |
| Preço | Cada nível custa ~3× o anterior (valores provisórios em `src/Upgrades.cs`) |
| Pagamento | Qualquer jogador compra com o dinheiro do time; o host confirma |
| Save | Arquivo separado do mod no PC do host; o save original não é alterado |
| Interface | Aba nova "Fábrica" na árvore de upgrades; se não der, os nós vão para o ramo Economy |

## Como o jogo funciona por dentro

Tudo fica no namespace `Il2CppMoonCleaners`. A lista completa de classes está em
[docs/assembly-csharp.txt](docs/assembly-csharp.txt), gerada por `tools/ClassDump`.

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
| `src/JonasModMain.cs` | Ponto de entrada do MelonLoader |
| `src/Upgrades.cs` | Definição dos 3 upgrades (ids, preços, multiplicadores) |
| `src/FactoryTree.cs` | Injeta o ramo "Fábrica" e os nós na `MoonUpgradeTree` |
| `src/Patches.cs` | Patches Harmony: velocidade das máquinas e multiplicador de dinheiro |

### Ainda não implementado

- Bloquear no lobby quem não tem o mod ou está em outra versão.
- Tirar os nós do mod do save original e guardar num arquivo separado.

## Compilar

Pré-requisitos: .NET SDK 6 ou mais novo, e o jogo aberto pelo menos uma vez com o MelonLoader
(isso gera `MelonLoader\Il2CppAssemblies`).

```
dotnet build -c Release
```

O build copia `JonasMod.dll` para `<pasta do jogo>\Mods`. Para usar outra pasta do jogo:
`dotnet build -p:GameDir="D:\caminho\Moon Sweepers"`.

Para regenerar a lista de classes depois de uma atualização do jogo:

```
dotnet run --project tools/ClassDump -c Release -- "<jogo>\MelonLoader\Il2CppAssemblies\Assembly-CSharp.dll" docs/assembly-csharp.txt
```

## Desinstalar o MelonLoader

Apague `version.dll`, `MelonLoader\`, `Mods\`, `Plugins\`, `UserData\` e `UserLibs\` da pasta do jogo.
O backup feito antes da instalação está em `Documents\MoonSweepers-backup-2026-10-27`.
