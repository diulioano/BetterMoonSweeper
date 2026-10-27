# Data Model: Ligar/desligar a bota de gravidade com Ctrl

A feature não cria dados novos nem salva nada. Ela só lê e muda estado que o jogo já tem.

## Estado da bota (do jogo)

| Campo | Onde | Uso |
|---|---|---|
| `OwnsGravityBoots` (bool, somente leitura) | `MoonWalker` | Condição de FR-003 |
| `GravityBootsOn` (bool) | `MoonWalker` | Estado atual; alternado por `ToggleGravityBoots()` |
| `GravityBootsOn` (bool) | `MoonRemotePlayer` | Como os outros jogadores veem o estado (US4) |

### Transições

```text
Desligada --Ctrl (com bota, sem bloqueio)--> Ligada
Ligada    --Ctrl (com bota, sem bloqueio)--> Desligada
qualquer  --Ctrl (sem bota OU bloqueado)---> sem mudança
```

"Bloqueado" é qualquer condição da tabela R4 em [research.md](research.md).

## Estado interno do mod

Só estado transitório, nunca salvo:

| Campo | Uso |
|---|---|
| `Keyboard` e dois `KeyControl` em cache | Evita criar wrappers IL2CPP a cada quadro; renovados se nulos ou em nova partida |
| Último valor de `MoonSession.InGame` | Detecta a entrada numa nova partida |
| Flag "exceção já logada" | `Tick()` loga só a primeira exceção |
| Flag "diagnóstico já rodou" (só com `DIAGNOSTICO`) | Diagnóstico de bindings roda uma vez |

Não é preciso guardar o estado anterior da tecla: `wasPressedThisFrame` já detecta o aperto.
