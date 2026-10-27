# Feature Specification: Mais níveis no Contrato Lunar

**Feature Branch**: `003-contrato-lunar-niveis`

**Created**: 2026-10-27

**Status**: Draft

**Input**: User description: "Gostaria de mais upgrades nos preços dos produtos." Esclarecimento do usuário: "continue com o contrato lunar, só que com mais upgrades".

> **O que já existe (v1)**: o Contrato Lunar (spec 001) tem 3 níveis e aumenta *todo* o dinheiro
> recebido pelo time em +10 / 25 / 50%. Esta feature acrescenta níveis ao mesmo upgrade. Upgrades
> por produto foram considerados e descartados pelo usuário.

> Nomes visíveis e ids substituídos pela spec 005 (textos em inglês; ids `bettermoonsweeper.*`).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Comprar níveis além do 3 (Priority: P1)

O time já tem o Contrato Lunar no nível 3 e continua comprando: aparecem os níveis 4, 5 e 6, cada
um aumentando mais o dinheiro recebido, pelo mesmo nó e do mesmo jeito que os níveis anteriores.

**Why this priority**: é o pedido inteiro.

**Independent Test**: com o Contrato Lunar no nível 3, comprar o nível 4 e vender a mesma carga
antes e depois.

**Acceptance Scenarios**:

1. **Given** Contrato Lunar nível 3, **When** o jogador abre o nó, **Then** ele mostra 6 níveis no total e oferece o nível 4 com o preço dele.
2. **Given** Contrato Lunar nível 4, **When** o time recebe dinheiro, **Then** recebe 1,75× o que o jogo pagaria sem o mod.
3. **Given** Contrato Lunar nível 6, **When** o time recebe dinheiro, **Then** recebe 2,5× o que o jogo pagaria sem o mod.
4. **Given** Contrato Lunar nível 6, **When** o jogador olha o nó, **Then** ele aparece no máximo e não pode ser comprado de novo.
5. **Given** o nó aberto, **When** o jogador olha o efeito, **Then** vê o texto do nível no formato "+75% de dinheiro".

---

### User Story 2 - Partidas em andamento continuam valendo (Priority: P1)

Um grupo que já comprou níveis do Contrato Lunar na versão anterior do mod atualiza o mod e continua
a partida: os níveis já comprados continuam lá, com o mesmo efeito, e os níveis novos ficam
disponíveis para compra.

**Why this priority**: perder o que foi comprado seria pior que não ter a feature.

**Independent Test**: carregar um save com Contrato Lunar nível 3 feito na versão anterior.

**Acceptance Scenarios**:

1. **Given** um save com Contrato Lunar nível 2 feito na versão anterior, **When** ele é carregado na versão nova, **Then** o nó está no nível 2, o efeito é +25%, e o próximo nível à venda é o 3.
2. **Given** um save com Contrato Lunar nível 3 feito na versão anterior, **When** ele é carregado na versão nova, **Then** o nível 4 está à venda.

### Edge Cases

- Os níveis 1 a 3 mantêm exatamente os mesmos multiplicadores e preços da v1.
- O Contrato Lunar multiplica todo o dinheiro recebido; no nível 6, isso inclui o dinheiro de qualquer fonte que o jogo já multiplica.
- Co-op: todos têm a mesma versão do mod, então host e clientes veem os mesmos 6 níveis e os mesmos preços.
- O preço do nível 6 é muito alto (regra ×3 da v1). Aceito: os números são provisórios e serão balanceados jogando.
- Gamepad: não há nada específico de controle nesta feature.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O Contrato Lunar MUST ter 6 níveis.
- **FR-002**: Os multiplicadores MUST ser 1,10 / 1,25 / 1,50 / 1,75 / 2,00 / 2,50 nos níveis 1 a 6 (os três primeiros iguais à v1).
- **FR-003**: Os preços MUST seguir a regra da v1 (cada nível custa 3× o anterior, base 2500): 2500 / 7500 / 22500 / 67500 / 202500 / 607500 (provisórios).
- **FR-004**: Os níveis novos MUST ser comprados pelo mesmo nó e pelo mesmo fluxo dos níveis 1 a 3 (dinheiro do time, host confirma). Se o jogo não aceitar mais de 3 níveis num nó, trocar isso por um segundo nó exige emendar esta spec antes.
- **FR-005**: O texto do efeito MUST funcionar para todos os 6 níveis.
- **FR-006**: Níveis comprados em versões anteriores do mod MUST continuar valendo depois da atualização.
- **FR-007**: Host e clientes MUST calcular o mesmo efeito para o mesmo nível.
- **FR-008**: A versão do mod MUST subir, porque o efeito econômico muda (constituição, princípio I).

### Key Entities

- **Contrato Lunar**: upgrade existente da aba Fábrica; passa de 3 para 6 níveis.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Com Contrato Lunar nível 4, uma venda rende entre 174% e 176% do valor sem o upgrade, medida nas mesmas condições (mesmo jogador, mesma estação, mesma carga).
- **SC-002**: Com Contrato Lunar nível 6, uma venda rende entre 249% e 251% do valor sem o upgrade, nas mesmas condições de SC-001.
- **SC-003**: Em 100% dos saves da versão anterior testados, o nível do Contrato Lunar é o mesmo depois da atualização.
- **SC-004**: É possível comprar os 6 níveis em sequência, e o 7º não aparece.
- **SC-005**: Em co-op com 2 jogadores, host e cliente veem o mesmo nível, o mesmo preço do próximo nível e o mesmo efeito.

## Assumptions

- "Mais upgrades" significa mais níveis no Contrato Lunar; o usuário confirmou.
- 6 níveis no total e os multiplicadores dos níveis 4 a 6 foram escolhidos por padrão; são provisórios.
- O nível de cada nó é guardado pelo id do nó, então aumentar o máximo não invalida níveis já comprados.
- A tela da árvore mostra qualquer quantidade de níveis num nó (hipótese a confirmar em jogo).
- Validar em jogo depende do MelonLoader voltar a executar mods na Unity 6.6.
