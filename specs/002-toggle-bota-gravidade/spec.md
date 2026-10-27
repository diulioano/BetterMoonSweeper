# Feature Specification: Ligar/desligar a bota de gravidade com Ctrl

**Feature Branch**: `002-toggle-bota-gravidade`

**Created**: 2026-10-27

**Status**: Draft

**Input**: User description: "Quando eu tiver o upgrade da bota de gravidade, quero apertar CTRL para ativar ou desativar ela."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Alternar a bota com uma tecla (Priority: P1)

Um jogador que já comprou a bota de gravidade aperta Ctrl uma vez e a bota liga; aperta de novo e
ela desliga. Ele ouve o mesmo som de ligar/desligar que o jogo já usa e não precisa segurar a tecla.

**Why this priority**: é o pedido inteiro; sem isso a feature não existe.

**Independent Test**: com a bota comprada, apertar Ctrl no chão da Lua e ver/ouvir a bota ligar;
apertar de novo e ver/ouvir desligar.

**Acceptance Scenarios**:

1. **Given** o jogador tem a bota e ela está desligada, **When** ele aperta Ctrl (esquerdo ou direito), **Then** a bota liga e toca o som de ligar.
2. **Given** a bota está ligada, **When** ele aperta Ctrl, **Then** a bota desliga e toca o som de desligar.
3. **Given** a bota está ligada, **When** ele solta o Ctrl, **Then** nada muda (a bota continua ligada até o próximo aperto).
4. **Given** o jogador segura o Ctrl por vários segundos, **When** ele finalmente solta, **Then** a bota trocou de estado só uma vez.

---

### User Story 2 - Sem efeito para quem não tem a bota (Priority: P1)

Um jogador que ainda não comprou a bota aperta Ctrl e nada acontece: nada liga, nenhum som toca e
nenhuma mensagem de erro aparece.

**Why this priority**: evita dar a bota de graça e evita confusão.

**Independent Test**: num save sem a bota, apertar Ctrl e ver que nada muda.

**Acceptance Scenarios**:

1. **Given** o jogador não tem a bota, **When** ele aperta Ctrl, **Then** a gravidade não muda e nenhum som toca.
2. **Given** o jogador compra a bota durante a sessão, **When** ele aperta Ctrl logo depois, **Then** a bota liga (sem reiniciar o jogo).

---

### User Story 3 - Não atrapalhar outras ações (Priority: P2)

Ctrl não liga a bota quando o jogador está digitando no chat, com um menu aberto, dirigindo um
veículo ou no modo de construção.

**Why this priority**: trocar a gravidade sem querer no meio de outra ação irrita, mas o jogo
continua jogável.

**Independent Test**: abrir o chat, digitar com Ctrl (por exemplo Ctrl+A), fechar o chat e
confirmar que a bota não mudou.

**Acceptance Scenarios**:

1. **Given** o chat está aberto, **When** o jogador aperta Ctrl, **Then** a bota não muda.
2. **Given** o menu de pausa ou a árvore de upgrades está aberta, **When** o jogador aperta Ctrl, **Then** a bota não muda.
3. **Given** o jogador está dirigindo ou é passageiro de um veículo, **When** ele aperta Ctrl, **Then** a bota não muda.
4. **Given** o jogador está no modo de construção, **When** ele aperta Ctrl, **Then** a bota não muda.

---

### User Story 4 - Os outros jogadores veem o estado certo (Priority: P2)

Em co-op, quando um jogador liga ou desliga a bota com Ctrl, os outros veem o efeito no personagem
dele (botas visíveis, forma de andar) do mesmo jeito que quando a bota é ligada pelo controle normal
do jogo.

**Why this priority**: mantém o co-op consistente (constituição, princípio I).

**Independent Test**: dois jogadores; um alterna a bota com Ctrl e o outro observa o personagem.

**Acceptance Scenarios**:

1. **Given** dois jogadores na mesma sessão, **When** o jogador A liga a bota com Ctrl, **Then** o jogador B vê o personagem de A com a bota ligada em até 1 segundo.
2. **Given** o jogador A aperta Ctrl, **When** a bota de A troca de estado, **Then** a bota do jogador B não muda.

### Edge Cases

- O jogo já tem uma tecla própria para a bota: ela continua funcionando como antes, e Ctrl é um atalho a mais.
- Ctrl é usado junto com outra tecla (Ctrl+C, Ctrl+V fora do chat): a bota alterna mesmo assim, porque o jogo não usa combinações com Ctrl durante a caminhada. Se isso mudar, a regra é revisada.
- O jogador está no ar (pulando ou com jetpack) quando aperta Ctrl: a bota alterna e a gravidade muda como o jogo faria com a tecla original.
- O jogador está caído ou desacordado (knocked): a bota não muda.
- O jogador está montado no robô aspirador, subindo escada, descansando, em viagem (teleporte/transição), sendo carregado por outro jogador ou no modo voo de dev: a bota não muda (tratado como "ocupado", igual a veículo).
- Controle (gamepad): fora de escopo; o controle continua com o botão que o jogo já usa.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Apertar Ctrl esquerdo ou Ctrl direito MUST alternar a bota de gravidade do jogador local entre ligada e desligada.
- **FR-002**: Cada aperto MUST alternar uma única vez; segurar a tecla não repete a troca.
- **FR-003**: O atalho MUST funcionar só se o jogador local tiver a bota; sem a bota, nada acontece e nada é exibido.
- **FR-004**: Ligar/desligar pelo atalho MUST ter o mesmo efeito, som e sincronização que ligar/desligar pela tecla original do jogo.
- **FR-005**: O atalho MUST ser ignorado enquanto o jogador digita, tem menu aberto, está num veículo ou robô, está no modo de construção, está caído, ou está em outro estado ocupado listado nos Edge Cases.
- **FR-006**: A tecla original da bota no jogo MUST continuar funcionando.
- **FR-007**: O atalho MUST afetar apenas a bota do jogador que apertou.

### Key Entities

- **Estado da bota**: ligada ou desligada, por jogador; já existe no jogo e é sincronizado por ele.
- **Posse da bota**: se o jogador comprou o upgrade da bota no ramo Suit da árvore do jogo.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em 20 apertos seguidos de Ctrl com a bota comprada, a bota alterna 20 vezes (100%).
- **SC-002**: O som e o estado da bota mudam no mesmo instante do aperto, sem atraso perceptível para quem testa.
- **SC-003**: Em 20 apertos de Ctrl sem a bota, a gravidade não muda nenhuma vez.
- **SC-004**: Em 10 apertos de Ctrl com o chat aberto, a bota não muda nenhuma vez.
- **SC-005**: Em co-op, o outro jogador vê a troca de estado em até 1 segundo em 100% dos testes.

## Assumptions

- A bota de gravidade já existe no jogo como upgrade comprável; a feature só adiciona o atalho.
- "CTRL" significa as duas teclas Ctrl do teclado, esquerda e direita.
- O jogo não usa Ctrl para outra ação enquanto se anda a pé; se usar, o comportamento será revisto em `research.md`.
- O atalho é fixo em Ctrl nesta versão; permitir trocar a tecla nas opções fica fora de escopo.
- O suporte a gamepad fica fora de escopo (confirmado pelo usuário).
- Validar em jogo depende do MelonLoader voltar a executar mods na Unity 6.6.
