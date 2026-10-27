# Quickstart: validar o atalho da bota de gravidade

## Pré-requisitos

- BepInEx 6 be.791 + patcher instalados e o mod carregado (ver "Instalar" no `README.md` e a spec 004).
- `dotnet build -c Release` sem erros (copia `BetterMoonSweeper.dll` para `<jogo>\Mods`).
- Um save onde o time já comprou a bota de gravidade (ramo Suit) e outro onde não comprou.
  Para chegar lá rápido, use o menu de dev do jogo (`DevAddMoney`) para ter dinheiro.

## Cenários

| # | Passos | Resultado esperado | Spec |
|---|---|---|---|
| 1 | Com a bota: apertar Ctrl esquerdo | Bota liga, som de ligar, log "ligada (Ctrl)" | US1-1, SC-002 |
| 2 | Apertar Ctrl direito | Bota desliga, som de desligar | US1-2 |
| 3 | Segurar Ctrl 3 s e soltar | Uma troca só | US1-3, US1-4 |
| 4 | Apertar Ctrl 20 vezes | 20 trocas | SC-001 |
| 5 | Sem a bota: apertar Ctrl 20 vezes | Nada muda, sem som, sem log | US2-1, SC-003 |
| 6 | Comprar a bota na sessão e apertar Ctrl | Bota liga | US2-2 |
| 7 | Abrir o chat, digitar com Ctrl 10 vezes, fechar | Bota não mudou | US3-1, SC-004 |
| 8 | Abrir pausa / árvore de upgrades, apertar Ctrl | Bota não muda | US3-2 |
| 9 | Entrar num veículo, apertar Ctrl | Bota não muda | US3-3 |
| 10 | Entrar no modo de construção, apertar Ctrl | Bota não muda | US3-4 |
| 11 | Usar a tecla nativa da bota | Funciona como antes | FR-006 |
| 12 | Co-op (2 jogadores): A aperta Ctrl, B observa | B vê a bota de A trocar em até 1 s; a bota de B não muda | US4, SC-005 |
| 13 | Montar no robô aspirador, apertar Ctrl | Bota não muda | FR-005, Edge Cases |
| 14 | Ficar caído (knocked) e apertar Ctrl; subir uma escada e apertar Ctrl | Bota não muda nos dois | FR-005, Edge Cases |
| 15 | Pular (ou usar o jetpack) e apertar Ctrl no ar | Bota alterna | Edge Cases |
| 16 | Fora do chat, apertar Ctrl+C | Bota alterna (comportamento aceito pela spec) | Edge Cases |

## Diagnóstico

- Log do mod: `<jogo>\BepInEx\LogOutput.log`.
- Se o cenário 1 ligar a bota sem som ou o 12 não sincronizar, seguir os fallbacks de R1 em
  [research.md](research.md).
