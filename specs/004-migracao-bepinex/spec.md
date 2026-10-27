# Feature Specification: Migração do mod para BepInEx 6

**Feature Branch**: `004-migracao-bepinex`

**Created**: 2026-10-27

**Status**: Draft

**Input**: User description: "Quero testar em jogo. Migrar para BepInEx 6, que é o que os mods do Nexus usam." Decisões do usuário: rota "BepInEx 6.0.0-be.791 + MoonSweepers BepInEx Patcher (Nexus mod 9)"; MelonLoader movido para o backup.

> **Contexto**: com o MelonLoader 0.7.3 o mod carrega, mas não executa (`No Support Module Loaded!`),
> então nada das specs 001 a 003 pôde ser testado. A comunidade do jogo usa BepInEx 6 be.791 mais um
> patcher próprio para a Unity 6000.6 / metadata 108, e vários mods do Nexus rodam assim.

> Nomes visíveis e ids substituídos pela spec 005 (textos em inglês; ids `bettermoonsweeper.*`).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - O mod roda no jogo (Priority: P1)

O jogador instala o BepInEx com o patcher, coloca o BetterMoonSweeper na pasta de plugins e abre o
jogo. O log mostra o mod carregado, e as features já existentes (aba Fábrica, máquinas mais rápidas,
Contrato Lunar com 6 níveis, Ctrl na bota de gravidade) funcionam como descrito nas specs 001 a 003.

**Why this priority**: sem isso nenhuma feature pode ser testada.

**Independent Test**: abrir o jogo e procurar no log a linha de carregamento do mod e a aba "Fábrica"
na árvore de upgrades.

**Acceptance Scenarios**:

1. **Given** BepInEx be.791 e o patcher instalados, **When** o jogo abre, **Then** o log termina com o carregamento completo dos plugins e inclui "BetterMoonSweeper 0.3.0 carregado".
2. **Given** o mod carregado, **When** o jogador abre a árvore de upgrades, **Then** a aba "Fábrica" aparece com os 3 nós.
3. **Given** o mod carregado e a bota comprada, **When** o jogador aperta Ctrl, **Then** a bota alterna (spec 002).

---

### User Story 2 - Amigos instalam sozinhos (Priority: P1)

Um amigo segue um guia curto no README e chega ao mesmo estado: BepInEx be.791, patcher, mod na
mesma versão, sem MelonLoader.

**Why this priority**: o mod é para co-op; todos precisam da mesma instalação (constituição I).

**Independent Test**: seguir o guia numa instalação limpa do jogo e conferir o log.

**Acceptance Scenarios**:

1. **Given** uma instalação limpa do jogo, **When** o amigo segue o guia, **Then** o log mostra o mod carregado na mesma versão do host.
2. **Given** um amigo com a instalação antiga (MelonLoader e `JonasMod.dll`), **When** ele segue o guia, **Then** o guia diz o que remover antes.

---

### User Story 3 - Desenvolvimento continua funcionando (Priority: P2)

O desenvolvedor compila o mod com um comando, e a DLL vai direto para a pasta de plugins. A lista de
classes do jogo pode ser regenerada a partir das DLLs do BepInEx.

**Why this priority**: necessário para continuar o mod, mas não muda nada para quem só joga.

**Independent Test**: `dotnet build -c Release` sem erros e com a DLL copiada para a pasta de plugins.

**Acceptance Scenarios**:

1. **Given** o jogo aberto uma vez com o patcher (DLLs do jogo geradas), **When** o desenvolvedor compila, **Then** a DLL vai para a pasta de plugins do BepInEx.
2. **Given** uma atualização do jogo, **When** o desenvolvedor regenera a lista de classes, **Then** `docs/assembly-csharp.txt` reflete os nomes que o mod usa.

### Edge Cases

- MelonLoader e BepInEx na mesma pasta: não suportado; o guia manda remover o MelonLoader.
- O patcher ou o BepInEx deixam de funcionar após uma atualização do jogo: o mod não carrega e o log diz isso; o jogo continua abrindo.
- O primeiro início demora mais (geração das DLLs do jogo): esperado.
- Diferença de nomes de namespace entre o MelonLoader e o BepInEx: o código e a documentação usam os nomes do BepInEx.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O mod MUST carregar como plugin do BepInEx 6.0.0-be.791 (IL2CPP, win-x64) com o MoonSweepers BepInEx Patcher.
- **FR-002**: Todas as funcionalidades implementadas nas specs 001 a 003 MUST continuar com o mesmo comportamento.
- **FR-003**: O mod MUST registrar no log do BepInEx uma linha com nome e versão ao carregar.
- **FR-004**: O README MUST ter um guia de instalação para jogadores (BepInEx, patcher, mod, o que remover da instalação antiga) e o procedimento para voltar ao jogo sem mods.
- **FR-005**: `dotnet build -c Release` MUST copiar a DLL para a pasta de plugins do BepInEx.
- **FR-006**: O projeto MUST deixar de depender de arquivos do MelonLoader.
- **FR-007**: A versão do mod MUST subir (constituição I), porque a instalação muda para todos.

### Key Entities

- **Instalação do jogador**: BepInEx be.791 + patcher + `BetterMoonSweeper.dll` em `BepInEx\plugins`.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em 3 inícios seguidos do jogo, o log mostra o mod carregado nas 3 vezes.
- **SC-002**: Todos os cenários dos quickstarts das specs 002 e 003, e os critérios SC-001 a SC-004 da spec 001, passam com o BepInEx.
- **SC-003**: Um amigo instala tudo seguindo só o README em menos de 10 minutos.
- **SC-004**: Remover os arquivos listados no guia de desinstalação devolve o jogo ao estado original (abre sem mods).

## Assumptions

- O patcher do Nexus funciona com o jogo na versão atual (patch 1.0.4, Unity 6000.6.0f1, metadata 108), conforme os relatos da comunidade.
- O usuário baixa o patcher manualmente do Nexus (exige login).
- A barreira de lobby e o save separado (spec 001) continuam pendentes e fora desta spec.
- Desde o patch 1.0.3 a economia de upgrades é decidida pelo host (fonte: guia da comunidade); isso não muda nesta spec, mas reforça que todos devem ter o mod.
