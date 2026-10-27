# Feature Specification: Textos do mod em inglês, no padrão do jogo

**Feature Branch**: `005-textos-ingles`

**Created**: 2026-10-27

**Status**: Draft

**Input**: User description: "Mantenha os textos com o padrão do jogo, em inglês." e "também mude a nomenclatura de jonas mod to BetterMoonSweeper".

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A aba do mod parece parte do jogo (Priority: P1)

O jogador abre a árvore de upgrades e vê a aba do mod, os nomes dos upgrades, os resumos e os
efeitos em inglês, com o mesmo tom e formato dos nós originais (frases curtas, efeitos como
"+10% launch payout").

**Why this priority**: o jogo é em inglês; textos em português destoam e confundem os amigos.

**Independent Test**: abrir a aba do mod e comparar os textos com os de um ramo original.

**Acceptance Scenarios**:

1. **Given** o mod carregado, **When** o jogador abre a árvore, **Then** a aba se chama "Factory" e o subtítulo está em inglês.
2. **Given** a aba aberta, **When** o jogador olha os 3 nós, **Then** nome, resumo e efeito de cada nível estão em inglês.
3. **Given** um Windows configurado em português, **When** o jogador lê os efeitos, **Then** nenhum tem vírgula: todos os percentuais são inteiros (por exemplo "+50%", "+150%").

### Edge Cases

- Mensagens de log do mod (só para o desenvolvedor) continuam em português.
- Os ids internos dos nós mudam de `jonasmod.*` para `bettermoonsweeper.*`. Níveis comprados com os ids antigos se perdem; aceito porque o mod só rodou em jogo pela primeira vez em 2026-10-27 e só em sessões de teste.
- O texto não pode ser maior que o dos nós originais a ponto de cortar na tela (verificar em jogo).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Todo texto do mod visível ao jogador (nome e subtítulo da aba, nome, resumo e efeito dos nós) MUST estar em inglês.
- **FR-002**: Os efeitos MUST seguir o formato dos nós do jogo: sinal, número, `%` e o que muda (por exemplo "+50% compactor speed", "+10% payout").
- **FR-003**: Números MUST ser formatados de forma independente do idioma do Windows (ponto decimal).
- **FR-004**: O comportamento dos upgrades (multiplicadores, preços, níveis) MUST continuar igual.
- **FR-005**: Nenhum nome do mod (código, ids dos nós, documentação atual) MUST usar "JonasMod"/"jonasmod"; os ids passam a ser `bettermoonsweeper.compactor`, `bettermoonsweeper.stonepress` e `bettermoonsweeper.contract`. Menções históricas (arquivos da instalação antiga) podem ficar.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Nenhuma palavra em português aparece na árvore de upgrades com o mod carregado.
- **SC-002**: Num Windows em português, nenhum efeito do mod mostra vírgula; todos os percentuais são inteiros. A formatação independente de idioma (FR-003) vale também para números futuros.
- **SC-003**: Níveis comprados depois da mudança continuam comprados ao salvar e carregar.

## Assumptions

- O estilo do jogo foi lido das strings do próprio jogo: resumos de uma frase ("Leg servos tuned for lunar gravity: you walk and run faster."), efeitos curtos ("+8% speed, +15% jump", "+10% launch payout"), nomes de máquinas "Dust Compactor" e "Stone press", subtítulos sem ponto final ("Earn more from every launch").
- A constituição muda: texto para o jogador em inglês; documentação, logs e comunicação em português.
