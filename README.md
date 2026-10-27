# BetterMoonSweeper

Mod para o **Moon Sweepers** (DragDrop Games) feito para jogar em co-op com amigos. Ele adiciona uma
aba nova de upgrades de fábrica, mais níveis para o lucro do time e um atalho para a bota de
gravidade. Os textos dentro do jogo seguem o padrão do próprio Moon Sweepers, em inglês.

| | |
|---|---|
| Versão | **0.3.1** |
| Jogo | Moon Sweepers patch 1.0.4 (Unity 6000.6, IL2CPP) |
| Loader | BepInEx 6.0.0-be.791 + Moon Sweepers BepInEx Patcher V1.0.0 |
| Multiplayer | Co-op; **todos** os jogadores precisam do mod na mesma versão |
| Estado | Carrega no jogo e injeta a aba; testes de gameplay em andamento (ver [Limitações](#limitações-conhecidas)) |

---

## O que o mod adiciona

### 1. Aba FACTORY na árvore de upgrades

Uma aba nova, laranja, ao lado dos ramos do jogo (Core, Containers, Cleaning, Suit, Logistics,
Economy), com o subtítulo *"Faster machines, bigger payouts"*. Os upgrades são comprados como
qualquer outro nó: com o dinheiro do time, e o host confirma.

| Upgrade | O que faz | Níveis | Preço de cada nível |
|---|---|---|---|
| **Turbo Compactor** | *"Overclocked motors: the dust compactor turns dust into sand faster."* | +50% / +100% / +200% compactor speed | 1.000 / 3.000 / 9.000 |
| **Turbo Stone Press** | *"Hotter kilns: the stone press bakes blocks faster."* | +50% / +100% / +200% stone press speed | 1.500 / 4.500 / 13.500 |
| **Lunar Contract** | *"A bigger cut of every sale: more money for the whole crew."* | +10% / +25% / +50% / +75% / +100% / +150% payout | 2.500 / 7.500 / 22.500 / 67.500 / 202.500 / 607.500 |

- **Turbo Compactor e Turbo Stone Press** encurtam o tempo de cada ciclo da máquina: +100% de
  velocidade significa ciclos com metade do tempo; +200%, com um terço.
- **Lunar Contract** multiplica **todo** o dinheiro que o time recebe, por cima dos bônus do próprio
  jogo (como o *Market Contracts*, do ramo Economy). Tem 6 níveis; o nível 6 é um objetivo de
  fim de jogo.
- Preços e porcentagens são provisórios e vão ser ajustados jogando.

### 2. Ctrl liga e desliga a bota de gravidade

Depois de comprar a bota de gravidade (ramo Suit), aperte **Ctrl** (esquerdo ou direito) para
alternar entre gravidade da Lua e da Terra, com qualquer ferramenta na mão. O atalho usa a mesma
troca que o jogo faz pelo controle original, então o som e a sincronização com os outros jogadores
devem ser os mesmos (ainda a confirmar em co-op).

| Funciona | Ignorado (para não trocar sem querer) |
|---|---|
| A pé, com ou sem ferramenta, no chão ou no ar | Chat aberto, menus, árvore de upgrades |
| Segurar Ctrl troca uma vez só | Dirigindo, de passageiro ou no robô aspirador |
| A tecla original da bota continua funcionando | Modo de construção, escada, caído, descansando, em viagem, sendo carregado |

Sem suporte a controle (gamepad): no controle, vale o botão original do jogo.

---

## Instalar

Todos do grupo precisam fazer igual, com a mesma versão do mod.

1. **Só se você tinha uma versão antiga (JonasMod / 0.1.0 / 0.2.0):** apague da pasta do jogo
   `version.dll`, `MelonLoader\`, `Mods\` (com a `JonasMod.dll`), `Plugins\`, `UserData\` e `UserLibs\`.
2. Baixe o **BepInEx 6.0.0-be.791**, versão `Unity.IL2CPP` win-x64, no site oficial
   ([builds.bepinex.dev](https://builds.bepinex.dev/projects/bepinex_be), arquivo
   `BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.791+b782da4.zip`). Extraia na pasta do jogo, ao lado do
   `MoonSweepers.exe`.
   (Steam → Moon Sweepers → Gerenciar → Procurar arquivos locais.)
3. Baixe o **Moon Sweepers BepInEx Patcher V1.0.0** no Nexus
   ([mod 9](https://www.nexusmods.com/moonsweepers/mods/9)) e extraia na pasta do jogo. Deve ficar
   `BepInEx\patchers\MoonSweepers.ModEngine\MoonSweepers.ModEngine.dll`
   (SHA-256 `b91bb4d4c12870abe79039a04173bbd5077322a8613bf90159a200a8370b1f94`).
4. Abra o jogo uma vez e feche. O primeiro início demora um pouco mais.
5. Copie `BetterMoonSweeper.dll` para `BepInEx\plugins`.
6. Abra o jogo e confira `BepInEx\LogOutput.log`: deve ter `BetterMoonSweeper 0.3.1 carregado.` e,
   no fim, `Chainloader startup complete`.
   As mensagens `Unsupported metadata version found! ... got 108` e
   `Class::Init signatures have been exhausted` são normais.

## Jogar com o mod num save que já existe

O mod funciona em qualquer sessão, nova ou antiga: a aba aparece assim que a sessão carrega.
Antes de usar num save de verdade:

1. **Backup.** Quem hospeda copia `%USERPROFILE%\AppData\LocalLow\DragDropGames\MoonSweepers` para
   outro lugar. É o host quem guarda o save e decide as compras de upgrade.
2. **Todos com o mod, mesma versão.** O mod ainda não barra quem entra sem ele; se só um tiver,
   a árvore e o dinheiro ficam diferentes entre os jogadores.
3. **Teste rápido, uma vez:** numa sessão descartável, compre um nível da aba FACTORY, salve e
   saia. Renomeie `BepInEx\plugins\BetterMoonSweeper.dll` para `.dll.off` e carregue essa sessão.
   Se abrir normalmente, o save continua utilizável sem o mod.

## Desinstalar

Apague da pasta do jogo: `winhttp.dll`, `doorstop_config.ini`, `.doorstop_version`, `changelog.txt`,
`dotnet\` e `BepInEx\`. O jogo volta ao original. (Para tirar só este mod e manter outros, apague
apenas `BepInEx\plugins\BetterMoonSweeper.dll`.)

## Limitações conhecidas

- **Testes em jogo em andamento.** O mod carrega e cria a aba; os efeitos (velocidade das
  máquinas, payout, Ctrl) ainda estão sendo conferidos. Roteiros em `specs/*/quickstart.md`.
- **Os níveis do mod ficam dentro do save do jogo.** O save separado ainda não foi feito; por
  isso o backup e o teste acima.
- **Sem bloqueio no lobby** para quem não tem o mod ou tem outra versão.
- **Preços e porcentagens provisórios.**
- Uma atualização grande do jogo (nova versão da Unity) pode exigir uma versão nova do BepInEx ou
  do patcher.

## Histórico de versões

| Versão | Mudanças |
|---|---|
| 0.3.1 | Textos em inglês no padrão do jogo (aba FACTORY); ids internos `bettermoonsweeper.*` |
| 0.3.0 | Migração do MelonLoader para BepInEx 6: primeira versão que roda no jogo |
| 0.2.0 | Novo nome (era JonasMod); atalho Ctrl da bota de gravidade; Lunar Contract com 6 níveis |
| 0.1.0 | Aba de fábrica com Turbo Compactor, Turbo Stone Press e Lunar Contract (3 níveis). Não rodava: o MelonLoader 0.7.3 não executa mods na Unity 6000.6 |

## Próximos passos

- Save separado dos níveis do mod e bloqueio no lobby (spec 001).
- Balancear preços e porcentagens jogando.
- Fase 2: **JONAS 2.0**, um modelo maior do Jonas (no código, `Joonas`: `MoonJoonasRig`,
  `MoonJoonasMotion`, `MoonJoonasGauge`).

---

## Para desenvolvedores

### Compilar

Pré-requisitos: .NET SDK 6 ou mais novo, e o jogo aberto pelo menos uma vez com o BepInEx e o patcher
(isso gera `BepInEx\interop`).

```
dotnet build -c Release
```

O build copia `BetterMoonSweeper.dll` para `<pasta do jogo>\BepInEx\plugins`. Feche o jogo antes:
enquanto ele roda, a DLL fica travada. Para outra pasta do jogo:
`dotnet build -p:GameDir="D:\caminho\Moon Sweepers"`.

Build de diagnóstico, com logs extras: `dotnet build -c Release -p:Diagnostico=true`. Depois rode o
build normal de novo, para a DLL sem diagnóstico ficar em `BepInEx\plugins`.

Regenerar a lista de classes do jogo depois de uma atualização:

```
dotnet run --project tools/ClassDump -c Release -- "<jogo>\BepInEx\interop\Assembly-CSharp.dll" docs/assembly-csharp.txt
```

### Estrutura do código

| Arquivo | O que faz |
|---|---|
| `src/BetterMoonSweeperMain.cs` | Ponto de entrada (plugin `BasePlugin` do BepInEx); aplica os patches Harmony |
| `src/Upgrades.cs` | Os 3 upgrades: ids, textos, efeitos, preços e multiplicadores (tudo de um upgrade fica aqui) |
| `src/FactoryTree.cs` | Injeta a aba FACTORY e os nós na `MoonUpgradeTree`, uma vez só, sem duplicar |
| `src/Patches.cs` | Patches Harmony: velocidade das máquinas (`StepProduction`), payout (`PayoutMultiplier`), chamada por quadro do atalho (`MoonWorld.Update`) |
| `src/GravityBootsHotkey.cs` | Atalho Ctrl: detecção da tecla, bloqueios e `MoonWalker.ToggleGravityBoots()` |
| `docs/assembly-csharp.txt` | Lista de classes e assinaturas do jogo (gerada por `tools/ClassDump`) |

### Como o jogo funciona por dentro

Tudo fica no namespace `MoonCleaners` (DLLs geradas em `BepInEx\interop`). O código do jogo é
IL2CPP: temos as assinaturas, não os corpos dos métodos.

**Máquinas de produção**

| No jogo | Classe visual | Spec (estática em `MoonWorld`) |
|---|---|---|
| Dust Compactor (poeira → areia) | `MoonPressureChamberView` | `ChamberSpec`, `PressureChamberKind` |
| Stone press (poeira + areia → bloco de pedra) | `MoonStonePressView` | `PressSpec`, `StonePressKind` |
| Painel solar (energia) | `MoonSolarPanelView` | — |

- As classes `*View` só animam. A lógica fica em `MoonWorld`: `StepProduction(dt)`, `CanStartCycle`,
  `TryStartCycle`, `FinishCycle`, `EmitProduct`. O estado de cada máquina é um `MoonProductionState`
  (`Progress`, `Running`, `Cycles`, `stores`), guardado em `MoonWorld.production`.
- `ProcessorSpec` tem `cycleSeconds`, `runningKw`, `slotCapacity` e `output`. O mod acelera as
  máquinas dividindo o `cycleSeconds` (lido uma vez do jogo, nunca acumulado).
- Em co-op, `WriteCrewProduction` e `ApplyCrewProduction` sincronizam a produção; o host é a autoridade.

**Árvore de upgrades**

- `MoonUpgradeTree` (estática): `Nodes`, `byId`, `BranchNames`, `BranchColors`, `BranchTaglines`, `Build()`.
- `MoonUpgradeTree.Node`: `id`, `title`, `summary`, `branch`, `glyph`, `maxLevel`, `prices`,
  `requires`, `position`, `effect` (`Func<int, string>`, texto do efeito por nível).
- Compra: `MoonWorld.TryGetTreePrice`, `TryPurchaseTreeNode(player, id, expectedLevel, ref message)`,
  `TreeLevel(player, id)`; níveis em `MoonWorld.treeLevels` (`player → id → nível`); em co-op,
  `CrewPurchase(player, PurchaseKind.TreeNode, id, level)`.
- Save: `MoonUpgradeTreeSave.Write/Read`. Tela: `MoonUpgradeTreeUI`.

**Dinheiro e bota de gravidade**

- `PayoutMultiplier(player)` e `PriceMultiplier(player)` (`Il2CppSystem.Decimal`); o Lunar Contract
  multiplica o resultado de `PayoutMultiplier`. Menu de dev: `DevAddMoney(playerId, dollars)`.
- `MoonWalker`: `OwnsGravityBoots`, `GravityBootsOn`, `ToggleGravityBoots()`; jogador local em
  `MoonRunOver.LocalWalker`; teclado em `MoonInput.PlayKeyboard`.

**Estilo dos textos:** os textos do jogo ficam no `global-metadata.dat`. Resumos de uma frase,
efeitos curtos ("+10% launch payout", "+8% speed, +15% jump"). Os textos do mod seguem esse padrão.

### Especificações (Spec Kit)

O projeto usa o [Spec Kit](https://github.com/github/spec-kit). Regras em
[.specify/memory/constitution.md](.specify/memory/constitution.md); cada feature tem spec, plano,
tarefas e roteiro de teste em `specs/`:

| Spec | Feature | Estado |
|---|---|---|
| [001](specs/001-fabrica-v1/spec.md) | Aba de fábrica (linha de base) | Parcial: falta bloqueio no lobby e save separado |
| [002](specs/002-toggle-bota-gravidade/spec.md) | Ctrl liga/desliga a bota de gravidade | Implementada, teste em jogo pendente |
| [003](specs/003-contrato-lunar-niveis/spec.md) | Lunar Contract com 6 níveis | Implementada, teste em jogo pendente |
| [004](specs/004-migracao-bepinex/spec.md) | Migração para BepInEx 6 | Implementada; carrega e injeta a aba |
| [005](specs/005-textos-ingles/spec.md) | Textos em inglês; ids `bettermoonsweeper.*` | Implementada, teste em jogo pendente |

Fluxo para features novas: `/speckit-specify` → `/speckit-plan` → `/speckit-tasks` →
`/speckit-analyze` (contexto fresco) → `/speckit-implement`.

### Backups locais

Em `Documents\MoonSweepers-backup-2026-10-27`: o jogo antes de qualquer loader, `melonloader\`
(instalação antiga), `Mods-antes-do-rename\` (`JonasMod.dll`) e `saves-antes-bepinex\`.
