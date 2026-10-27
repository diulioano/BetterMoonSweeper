# Quickstart: validar os níveis novos do Contrato Lunar

## Pré-requisitos

- BepInEx 6 be.791 + patcher instalados e o mod carregado (ver "Instalar" no `README.md` e a spec 004).
- `dotnet build -c Release` sem erros.
- Dinheiro pelo menu de dev do jogo (`DevAddMoney`); o nível 6 custa 607.500.
- Cenário 6: as versões 0.1.0 e 0.2.0 nunca rodaram em jogo (MelonLoader bloqueado), então não existem saves com níveis delas. O cenário vira: salvar a sessão descartável com Contrato Lunar nível 2 e 3, sair e carregar de novo.

## Condições de medição (SC-006)

Mesmo jogador vendendo, mesma estação, mesma quantidade e tipo de carga, depositada depois da
compra do nível.

## Cenários

| # | Passos | Resultado esperado | Spec |
|---|---|---|---|
| 1 | Abrir a aba FACTORY e o nó Lunar Contract (spec 005) | 6 níveis; marcadores e número romano legíveis; anotar se algum indicador de progresso da árvore mudou | US1-1, R3, R5b |
| 2 | Comprar níveis 1 a 3 | Mesmos preços e textos da v1 (ou os mesmos com o desconto do jogo, se houver; research H4) | Edge case |
| 3 | Comprar o nível 4 | Preço 3× o do nível 3 (67.500 sem desconto); texto "+75% payout" | US1-1, US1-5, H4 |
| 4 | Vender uma carga no nível 3 e outra igual no nível 4 | Nível 4 rende entre 174% e 176% do valor sem upgrade | US1-2, SC-001 |
| 5 | Comprar até o nível 6 e vender | Entre 249% e 251%; nó no máximo, sem 7º nível | US1-3, US1-4, SC-002, SC-004 |
| 6 | Salvar a sessão com Contrato Lunar nível 2 (e depois 3), sair e carregar de novo | Nível preservado; próximo nível à venda (3 ou 4) | US2, SC-003 |
| 7 | Co-op (2 jogadores): comparar o nó no host e no cliente | Mesmo nível, preço e efeito | FR-007, SC-005 |

## Se algo falhar

- Nível 4 não comprável: fallback A (R2 em [research.md](research.md)); se não resolver, fallback B (R1), só depois de emendar a spec.
- Marcadores apertados com 6 níveis: cosmético e aceito (R3).
