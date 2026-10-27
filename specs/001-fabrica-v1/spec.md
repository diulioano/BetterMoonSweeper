# Feature Specification: Fábrica v1 (linha de base)

**Feature Branch**: `001-fabrica-v1`

**Created**: 2026-10-27

**Status**: Parcialmente implementada. O código das US1 a US3 compila mas nunca rodou em jogo (MelonLoader bloqueado na Unity 6.6). US4 e US5 não foram implementadas.

**Input**: User description: "Documentar como linha de base o que foi pedido na v1 do BetterMoonSweeper: ramo 'Fábrica' na árvore de upgrades com Compactador Turbo, Prensa Turbo e Contrato Lunar; compra com o dinheiro do time; save separado; bloqueio no lobby de quem não tem o mod."

> Esta spec registra o design acordado na Fase 1 (ver `README.md`) e o estado atual dele. Ela não
> pede trabalho novo: o que falta está marcado em "Status por requisito".

> Nomes visíveis e ids substituídos pela spec 005 (textos em inglês; ids `bettermoonsweeper.*`).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Comprar upgrades de fábrica num ramo próprio (Priority: P1)

Um jogador abre a árvore de upgrades e encontra uma aba nova "Fábrica" (laranja, subtítulo
"Máquinas mais rápidas, mais lucro."). Nela há três upgrades de 3 níveis cada. Ele compra um nível
com o dinheiro do time, como em qualquer outro nó da árvore.

**Why this priority**: sem a aba e os nós, nenhum dos efeitos da v1 fica acessível.

**Independent Test**: abrir a árvore, achar a aba "Fábrica", comprar o nível 1 do Compactador Turbo
e ver o saldo do time cair pelo preço do nível.

**Acceptance Scenarios**:

1. **Given** uma sessão com o mod, **When** o jogador abre a árvore de upgrades, **Then** a aba "Fábrica" aparece com Compactador Turbo, Prensa Turbo e Contrato Lunar.
2. **Given** o time tem dinheiro suficiente, **When** um jogador compra um nível, **Then** o preço daquele nível sai do dinheiro do time e o nível do nó sobe 1.
3. **Given** o nó está no nível 3, **When** o jogador olha o nó, **Then** ele aparece como no máximo e não pode ser comprado de novo.
4. **Given** a tela da árvore não suporta uma 7ª aba, **When** a opção de ramo próprio for desligada, **Then** os três nós aparecem no ramo Economy.

---

### User Story 2 - Máquinas mais rápidas (Priority: P1)

Com Compactador Turbo ou Prensa Turbo comprados, o compactador (poeira → areia) e a prensa de pedra
(poeira + areia → bloco) terminam cada ciclo mais rápido: 1,5×, 2× e 3× nos níveis 1, 2 e 3.

**Why this priority**: é o efeito principal do tema "fábrica".

**Independent Test**: cronometrar um ciclo do compactador antes e depois de comprar o nível 1; o
tempo deve cair para 2/3 do original.

**Acceptance Scenarios**:

1. **Given** Compactador Turbo nível 0, **When** o compactador roda, **Then** o ciclo dura o tempo original do jogo.
2. **Given** Compactador Turbo nível 2, **When** o compactador roda, **Then** o ciclo dura metade do tempo original.
3. **Given** Prensa Turbo nível 3, **When** a prensa roda, **Then** o ciclo dura 1/3 do tempo original.
4. **Given** uma sessão co-op, **When** o host e um cliente olham a mesma máquina, **Then** os dois veem a mesma velocidade e a mesma produção.

---

### User Story 3 - Mais dinheiro com o Contrato Lunar (Priority: P2)

Com o Contrato Lunar, todo dinheiro que o time recebe aumenta em +10%, +25% ou +50% (níveis 1 a 3;
a spec 003 acrescenta os níveis 4 a 6),
somando-se aos multiplicadores que o jogo já aplica.

**Why this priority**: é um bônus de economia; o mod é útil sem ele.

**Independent Test**: vender a mesma quantidade de poeira antes e depois do nível 1 e comparar os
valores recebidos (+10%).

**Acceptance Scenarios**:

1. **Given** Contrato Lunar nível 0, **When** o time recebe dinheiro, **Then** o valor é o do jogo sem o mod.
2. **Given** Contrato Lunar nível 3, **When** o time recebe dinheiro, **Then** o valor é 1,5× o que o jogo pagaria.

---

### User Story 4 - Só joga junto quem tem o mod (Priority: P2)

Quando alguém sem o mod, ou com outra versão, tenta entrar no lobby de um host com o mod, a entrada
é recusada com uma mensagem que diz o motivo e a versão exigida.

**Why this priority**: protege a sessão contra dessincronia (constituição, princípio I).

**Independent Test**: entrar com um cliente sem o mod num host com o mod e ver a recusa.

**Acceptance Scenarios**:

1. **Given** host com BetterMoonSweeper 0.1.0, **When** um cliente sem o mod tenta entrar, **Then** a entrada é recusada com a mensagem "Este lobby exige BetterMoonSweeper 0.1.0".
2. **Given** host com BetterMoonSweeper 0.1.0, **When** um cliente com BetterMoonSweeper 0.2.0 tenta entrar, **Then** a entrada é recusada com a mesma mensagem.
3. **Given** host e cliente com BetterMoonSweeper 0.1.0, **When** o cliente entra, **Then** a entrada é aceita normalmente.

---

### User Story 5 - Save original intocado (Priority: P2)

Os níveis dos upgrades do mod ficam num arquivo próprio no PC do host. O save do jogo continua
abrindo sem o mod, apenas sem os upgrades da Fábrica.

**Why this priority**: os saves estão na Steam Cloud; um save que só abre com o mod prende o grupo
ao mod (constituição, princípio III).

**Independent Test**: comprar um upgrade, salvar, remover o mod, carregar o save e confirmar que o
jogo abre sem erro.

**Acceptance Scenarios**:

1. **Given** níveis do mod comprados, **When** o jogo salva, **Then** os níveis do mod vão para o arquivo do mod e não aparecem no save do jogo.
2. **Given** um save feito com o mod, **When** ele é carregado com o mod, **Then** os níveis do mod voltam como estavam.
3. **Given** um save feito com o mod, **When** ele é carregado sem o mod, **Then** o jogo carrega sem erro e sem os nós da Fábrica.

### Edge Cases

- A árvore é montada mais de uma vez (voltar ao menu e entrar de novo): os nós não podem ser duplicados.
- O nível de um upgrade fica guardado por jogador ou pelo time? Até confirmar em jogo, vale o maior nível encontrado.
- O tempo de ciclo original da máquina muda numa atualização do jogo: o mod lê o valor do jogo, não guarda um número fixo.
- O host sai da sessão: o arquivo do mod fica no PC do host; o novo host não tem esses níveis (limitação aceita).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O mod MUST adicionar à árvore de upgrades uma aba "Fábrica" com os nós Compactador Turbo, Prensa Turbo e Contrato Lunar.
- **FR-002**: O mod MUST oferecer a opção de colocar esses nós no ramo Economy em vez de uma aba própria.
- **FR-003**: Compactador Turbo e Prensa Turbo MUST ter 3 níveis; o Contrato Lunar MUST ter os níveis definidos na spec 003 (6). Cada nível MUST custar 3× o anterior (base: 1000, 1500 e 2500).
- **FR-004**: A compra MUST usar o dinheiro do time e o fluxo nativo de compra da árvore (qualquer jogador compra, o host confirma).
- **FR-005**: Compactador Turbo e Prensa Turbo MUST dividir o tempo de ciclo da máquina por 1,5 / 2 / 3 conforme o nível.
- **FR-006**: Contrato Lunar MUST multiplicar o dinheiro recebido por 1,10 / 1,25 / 1,50 nos níveis 1 a 3 (níveis seguintes na spec 003), sobre os multiplicadores do jogo.
- **FR-007**: Host e clientes MUST calcular o mesmo efeito para o mesmo nível.
- **FR-008**: O mod MUST recusar no lobby jogadores sem o mod ou com outra versão, com mensagem que diga a versão exigida.
- **FR-009**: Os níveis do mod MUST ser salvos fora do save do jogo, no PC do host, e restaurados ao carregar.
- **FR-010**: Injetar os nós MUST ser idempotente.

### Status por requisito

| Requisito | Estado |
|---|---|
| FR-001, FR-002, FR-010 | Implementado em `src/FactoryTree.cs`, não testado em jogo; a posição dos nós na tela é provisória |
| FR-003 | Implementado em `src/Upgrades.cs`; preços provisórios. Contrato Lunar com 6 níveis desde a 0.2.0 (spec 003) |
| FR-004 | Usa o fluxo nativo, que os nós injetados herdam; não testado em jogo |
| FR-005, FR-007 | Implementado em `src/Patches.cs` (`StepProductionPatch`), não testado em jogo |
| FR-006 | Implementado em `src/Patches.cs` (`PayoutMultiplierPatch`), não testado em jogo; níveis 4 a 6 vêm da spec 003, sem mudança no patch |
| FR-008 | **Não implementado** (TODO em `src/BetterMoonSweeperMain.cs`) |
| FR-009 | **Não implementado**: hoje os níveis do mod entram no save do jogo pelo fluxo nativo, o que viola o princípio III |

### Key Entities

- **Upgrade do mod**: id do nó, nome, resumo, ícone, multiplicador por nível (3 níveis), preço base.
- **Nível do upgrade**: número de 0 a 3 por nó, compartilhado pelo time.
- **Arquivo de save do mod**: níveis dos nós do mod, ligados à sessão salva do host.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em 100% das aberturas da árvore, a aba "Fábrica" aparece com exatamente 3 nós, sem duplicatas.
- **SC-002**: Com Compactador Turbo nível 2, o ciclo cronometrado fica entre 45% e 55% do tempo original.
- **SC-003**: Com Contrato Lunar nível 1, uma venda rende entre 109% e 111% do valor sem o upgrade.
- **SC-004**: Em co-op com 2 jogadores, após 10 minutos de produção, host e cliente mostram a mesma quantidade produzida.
- **SC-005**: Um cliente sem o mod é recusado em 100% das tentativas de entrar.
- **SC-006**: Um save feito com o mod abre sem erro no jogo sem o mod.

## Assumptions

- Todos os jogadores do grupo instalam o mod (constituição, princípio I).
- O fluxo nativo de compra (`TryPurchaseTreeNode` / `CrewPurchase`) aceita nós injetados sem mudanças.
- Os preços e os multiplicadores são provisórios e serão balanceados jogando.
- Testar em jogo depende do MelonLoader voltar a executar mods na Unity 6.6 (LavaGang/MelonLoader#1180).
- JONAS 2.0 (modelo maior do Jonas, `MoonJoonasRig`) é a Fase 2 e está fora desta spec.
