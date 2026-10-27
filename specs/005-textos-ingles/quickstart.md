# Quickstart: validar os textos em inglês

## Pré-requisitos

- BepInEx 6 be.791 + patcher + BetterMoonSweeper 0.3.1 instalados (README, "Instalar").
- Windows configurado em português (para SC-002).
- Sessão nova descartável; dinheiro pelo menu de dev (`DevAddMoney`).

## Cenários

| # | Passos | Resultado esperado | Spec |
|---|---|---|---|
| 1 | Abrir a árvore de upgrades | Uma única aba "Factory", subtítulo "Faster machines, bigger payouts" | US1-1 |
| 2 | Olhar os 3 nós | "Turbo Compactor", "Turbo Stone Press", "Lunar Contract", com resumos em inglês e sem corte na tela | US1-2, SC-001 |
| 3 | Comprar e olhar o efeito de cada nível | Compactor: +50% / +100% / +200% compactor speed; Stone Press: +50% / +100% / +200% stone press speed; Lunar Contract: +10% / +25% / +50% / +75% / +100% / +150% payout | FR-002, SC-002 |
| 4 | Olhar o efeito de um nó antes de comprar o 1º nível | Anotar o texto mostrado; se for "+0% ...", decidir se fica assim | Edge case (`effect(0)`) |
| 5 | Comparar a pontuação dos resumos com a de um nó original | Mesma regra de ponto final | Assumptions |
| 6 | Comprar um nível, salvar, sair e carregar | Nível preservado | FR-004, SC-003 |
