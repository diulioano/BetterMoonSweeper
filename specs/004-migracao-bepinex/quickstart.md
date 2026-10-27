# Quickstart: validar a migração para BepInEx

## Pré-requisitos

- BepInEx 6.0.0-be.791 (Unity.IL2CPP win-x64) extraído na pasta do jogo.
- `BepInEx\patchers\MoonSweepers.ModEngine\MoonSweepers.ModEngine.dll` (Nexus mod 9).
- Nenhum arquivo do MelonLoader na pasta do jogo.

## Cenários

| # | Passos | Resultado esperado | Spec |
|---|---|---|---|
| 1 | Abrir o jogo pela primeira vez com o patcher e ficar no menu principal | Demora mais; `BepInEx\LogOutput.log` termina com `Chainloader startup complete`; `BepInEx\interop` com as DLLs do jogo | FR-001 |
| 2 | `dotnet build -c Release` | Sem erros; `BepInEx\plugins\BetterMoonSweeper.dll` atualizado | FR-005, US3 |
| 3 | Abrir o jogo de novo | Log com `BetterMoonSweeper 0.3.0 carregado` | FR-003, US1-1 |
| 4 | Repetir o cenário 3 mais 2 vezes | Carregado nas 3 vezes | SC-001 |
| 5 | Criar uma sessão nova descartável (saves reais com backup) e abrir a árvore de upgrades | Uma única aba "Factory" (spec 005) com 3 nós; Contrato Lunar com 6 níveis | US1-2, specs 001/003 |
| 6 | Com a bota comprada, apertar Ctrl | Bota alterna; log `Bota de gravidade: ligada (Ctrl)` | US1-3, spec 002 |
| 7 | Rodar os quickstarts das specs 002 e 003 | Todos passam | SC-002 |
| 8 | Desinstalar seguindo o README e abrir o jogo | Jogo abre sem mods | SC-004 |

## Log

`<jogo>\BepInEx\LogOutput.log`. Mensagens esperadas e inofensivas: `Unsupported metadata version
found! We support 23-106, got 108` e `Class::Init signatures have been exhausted`.
