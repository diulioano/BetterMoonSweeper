# Research: Mais níveis no Contrato Lunar

Fonte dos símbolos: `docs/assembly-csharp.txt`. Só temos assinaturas (IL2CPP); hipóteses marcadas.

## R1. Como aumentar o número de níveis

- **Decision**: estender o array `Multiplicadores` do `ContratoLunar` em `src/Upgrades.cs` de 3
  para 6 valores (`1.10, 1.25, 1.50, 1.75, 2.00, 2.50`).
- **Rationale**: o resto já deriva desse array. `UpgradeDef.NivelMaximo` é `Multiplicadores.Length`;
  `FactoryTree.CriarNo` monta `maxLevel` e `prices` a partir de `NivelMaximo` e `PrecoDoNivel`;
  o texto do efeito usa `Multiplicador(nivel)`; e `PayoutMultiplierPatch` usa o mesmo método. Não é
  preciso mexer em patch nem na injeção (princípio IV: tudo do upgrade em `Upgrades.cs`).
- **Alternatives considered**:
  - *Nó novo "Contrato Lunar II"* que exige o Contrato Lunar nível 3 (`Node.requires`, tupla
    `(id, nível)`). Mais trabalho, um segundo nó na tela e mudança no `PayoutMultiplierPatch` para
    combinar os dois nós. Vira o **fallback B**, usado só se o fallback A (R2) não resolver. Como
    contraria a spec FR-004 ("mesmo nó"), aplicá-lo exige emendar a spec antes (`/speckit-clarify`).

## R2. A compra respeita `node.maxLevel`?

- A compra passa por `MoonWorld.TryPurchaseTreeNode(player, id, expectedLevel, ref message)` e há
  um `MoonWorld.TreeMaxLevel(Node node)` separado.
- **Hipótese H1**: `TreeMaxLevel` devolve `node.maxLevel` (pode ser limitado por outra regra, por
  exemplo ferramenta ou modo de jogo).
- **Fallback A**: se o nível 4 não for comprável, Postfix em `TreeMaxLevel` devolvendo
  `def.NivelMaximo` para os nós do mod. Se ainda assim falhar, fallback B (R1).

## R3. A tela mostra 6 níveis?

- `MoonUpgradeTreeUI.NodeView` tem `pips` (RectTransform), `level` e `max`; os kits de UI montam os
  marcadores com `Pips(parent, x, y, count)`/`SetPips(row, level, max, ...)`, e o nível é escrito em
  algarismos romanos (`MoonWorld.Roman(int)`, estático).
- **Hipótese H2**: os marcadores são criados pela contagem `max`, sem limite fixo de 3. Se ficarem
  sobrepostos ou cortados com 6, é só cosmético e aceito; não justifica o fallback B sozinho.

## R4. Saves e partidas em andamento (FR-006)

- O nível fica em `MoonWorld.treeLevels` por id de nó (`jonasmod.contract`), sem referência ao
  máximo. Aumentar o máximo não muda o valor guardado, então um save com nível 3 continua nível 3.
- **Hipótese H3**: o carregamento do save não descarta níveis maiores que o máximo nem recalcula
  nada a partir do máximo. Irrelevante aqui, porque o máximo só cresce.
- A pendência de salvar fora do save do jogo (spec 001, FR-009) continua valendo, sem mudança.

## R5. Preços

- **Hipótese H4**: o preço cobrado vem de `MoonWorld.TryGetTreePrice(player, id, ref Decimal)`,
  que é por jogador. Ele pode aplicar um ajuste (existe `MoonWorld.PriceMultiplier(player)`, um
  desconto do próprio jogo). Por isso o quickstart compara a **proporção** entre níveis (×3) e não
  só os valores absolutos de `prices`.

- `PrecoDoNivel(n) = PrecoBase * (int)Math.Pow(3, n - 1)`: com base 2500, o nível 6 custa 607.500,
  dentro de `int`. Mantida a regra ×3 da v1; os valores são provisórios e aceitos pelo usuário.

## R5b. Totais da árvore

- `MoonWorld.TreeLevelsTotal()` e `TreeLevelsOwned(player)` somam níveis da árvore. Com 3 níveis a
  mais, o total cresce, o que pode mudar algum indicador de progresso ou conquista do jogo. Isso
  já acontece desde a v1 (nós injetados entram no total). **Risco aceito**, só observado no cenário 1
  do quickstart.

## R6. Versão

- Muda o efeito econômico e o máximo do nó, que precisam ser iguais em todos os jogadores. A regra
  de versão é a mesma da 002 (`specs/002-toggle-bota-gravidade/research.md`, R7): subir para uma
  versão que nenhum build distribuído usou.
