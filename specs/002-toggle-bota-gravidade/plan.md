# Implementation Plan: Ligar/desligar a bota de gravidade com Ctrl

**Branch**: `002-toggle-bota-gravidade` | **Date**: 2026-10-27 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/002-toggle-bota-gravidade/spec.md`

## Summary

Atalho Ctrl (esquerdo ou direito) que alterna a bota de gravidade do jogador local quando ele já a
comprou. A cada quadro, `BetterMoonSweeperMain.OnUpdate` chama `GravityBootsHotkey.Tick()`, que detecta o
aperto, confere as condições de bloqueio e chama o método nativo `MoonWalker.ToggleGravityBoots()`.
Som e sincronização ficam com o jogo. Ver [research.md](research.md).

## Technical Context

**Language/Version**: C# (LangVersion latest), net6.0

**Primary Dependencies**: MelonLoader 0.7.3, Il2CppInterop, `Assembly-CSharp.dll` gerado (namespace `Il2CppMoonCleaners`), `Unity.InputSystem.dll` gerado (`Keyboard`, `KeyControl`, `Key`)

**Storage**: N/A (nada é salvo)

**Testing**: manual em jogo, pelo [quickstart.md](quickstart.md) (constituição, princípio VI); não há testes automatizados

**Target Platform**: Windows, Moon Sweepers (Unity 6000.6, IL2CPP)

**Project Type**: mod (DLL única carregada pelo MelonLoader)

**Performance Goals**: checagem por quadro em O(1). `Keyboard` e os dois `KeyControl` do Ctrl ficam em cache e só são renovados quando o cache está nulo ou quando `MoonSession.InGame` passa de falso para verdadeiro (nova partida). Não comparar `PlayKeyboard` por referência a cada quadro: no Il2CppInterop cada leitura cria um wrapper novo

**Constraints**: símbolos do jogo só se presentes em `docs/assembly-csharp.txt`; APIs da Unity via `Unity.InputSystem.dll`; nenhum efeito em jogadores sem o mod

**Scale/Scope**: 1 arquivo novo, 1 método novo em `BetterMoonSweeperMain`, 1 propriedade de build (`Diagnostico`) em `BetterMoonSweeper.csproj`

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Princípio | Situação |
|---|---|
| I. Co-op só com o mod | ✅ Usa o estado nativo da bota, já sincronizado pelo jogo; a feature não muda nada para quem não tem o mod. Barrar no lobby quem não tem o mod é outra feature, ainda pendente (spec 001, FR-008) |
| II. Instalação protegida | ✅ Só a DLL vai para `Mods` |
| III. Save intocado | ✅ Nada é salvo |
| IV. Patches mínimos | ✅ Nenhum patch Harmony; só leitura de input e uma chamada nativa |
| V. Símbolos rastreáveis | ✅ Todos listados em research.md R1–R4; hipóteses marcadas |
| VI. Validação em jogo | ⚠️ Depende do desbloqueio do MelonLoader; a feature fica "implementada, não testada" até lá |

Reavaliação pós-design: sem mudanças.

## Project Structure

### Documentation (this feature)

```text
specs/002-toggle-bota-gravidade/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── atalho-teclado.md
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── BetterMoonSweeperMain.cs        # + OnUpdate() → GravityBootsHotkey.Tick()
└── GravityBootsHotkey.cs  # novo: detecção do Ctrl, bloqueios e chamada nativa
BetterMoonSweeper.csproj            # + DIAGNOSTICO quando -p:Diagnostico=true
```

**Structure Decision**: projeto único existente; a feature fica isolada em `src/GravityBootsHotkey.cs`,
seguindo o padrão de um arquivo por assunto (`FactoryTree.cs`, `Patches.cs`).

## Design

```text
BetterMoonSweeperMain.OnUpdate()
  └─ GravityBootsHotkey.Tick()
       1. se !MoonSession.InGame → return
       2. aperto = KeyControls em cache (renovado se nulo ou em nova partida) leftCtrl/rightCtrl .wasPressedThisFrame
       3. se !aperto → return
       4. se MoonInput.Typing || MoonFramePacing.MenuOpen() → return
       5. walker = MoonRunOver.LocalWalker; se null → return
       6. se !walker.OwnsGravityBoots || MenuBlocked || UIBlocked
             || Mounted || Passenger || Riding != null || Building || MoonInput.BuildToolInHand
             || Knocked || Climbing || Resting || Travelling || Held || Flying → return
       7. ok = walker.ToggleGravityBoots(); se ok → log "Bota de gravidade: ligada/desligada (Ctrl)";
          senão → log "ToggleGravityBoots retornou false" (valida a hipótese de R1)
```

- Fora de partida, o passo 1 sai sem ler o teclado; em partida, o caso comum (nenhum aperto) sai no
  passo 3, lendo só os dois `KeyControl` em cache.
- Exceções em `Tick()` são capturadas e logadas uma vez só (flag), para não inundar o log se algum
  símbolo mudar numa atualização do jogo.

## Complexity Tracking

Nenhuma violação a justificar.
