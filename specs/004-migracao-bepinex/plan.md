# Implementation Plan: Migração do mod para BepInEx 6

**Branch**: `004-migracao-bepinex` | **Date**: 2026-10-27 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/004-migracao-bepinex/spec.md`

## Summary

Trocar o MelonLoader pelo BepInEx 6.0.0-be.791 com o patcher da comunidade para a Unity 6000.6. O
mod vira um `BasePlugin` do BepInEx que aplica os patches Harmony já existentes; o atalho Ctrl passa
a ser chamado por um Postfix em `MoonWorld.Update()` (sem class injection). O projeto passa a
referenciar `BepInEx\core` e `BepInEx\interop` e a copiar a DLL para `BepInEx\plugins`. Os
`using` são ajustados depois de ver os nomes gerados. Ver [research.md](research.md).

## Technical Context

**Language/Version**: C# (LangVersion latest), net6.0

**Primary Dependencies**: BepInEx 6.0.0-be.791 (Unity.IL2CPP win-x64), MoonSweepers BepInEx Patcher (Nexus mod 9), HarmonyX (`0Harmony.dll` do BepInEx), Il2CppInterop do BepInEx, DLLs de interop geradas em `BepInEx\interop`

**Storage**: N/A

**Testing**: manual em jogo, pelo [quickstart.md](quickstart.md) e pelos quickstarts das specs 002 e 003

**Target Platform**: Windows, Moon Sweepers patch 1.0.4 (Unity 6000.6.0f1, IL2CPP, metadata 108)

**Project Type**: mod (DLL única, plugin do BepInEx)

**Performance Goals**: sem mudança; o Postfix em `MoonWorld.Update` faz o mesmo trabalho que o `OnUpdate` fazia

**Constraints**: símbolos do jogo conferidos no dump regenerado a partir do BepInEx; nada fora de `BepInEx\plugins` é alterado pelo build

**Scale/Scope**: `BetterMoonSweeper.csproj`, `src/BetterMoonSweeperMain.cs`, `src/GravityBootsHotkey.cs`, `src/FactoryTree.cs`, `src/Patches.cs`, `src/Upgrades.cs` (usings), `docs/assembly-csharp.txt`, README, CLAUDE.md, constituição, quickstarts 002/003

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Princípio | Situação |
|---|---|
| I. Co-op só com o mod | ⚠️ Versão sobe para 0.3.0 e o guia padroniza a instalação; a barreira de lobby continua pendente (spec 001) |
| II. Instalação protegida | ✅ Usuário autorizou baixar o BepInEx e mexer na pasta do jogo; MelonLoader movido para o backup. O texto do princípio II cita `Mods`: emendar para `BepInEx\plugins` |
| III. Save intocado | ⚠️ Herdado da v1 e agora com efeito real; justificado no Complexity Tracking, com backup e sessão descartável |
| IV. Patches mínimos | ✅ Mesmos patches; só um Postfix novo, para chamar o atalho. `Injetar()` fica idempotente mesmo com falha parcial (T013, research H5) |
| V. Símbolos rastreáveis | ✅ Dump regenerado a partir das DLLs do BepInEx (T007) |
| VI. Validação em jogo | ✅ Passa a ser possível; é o objetivo da feature |

Emenda da constituição (MINOR → 1.1.0), feita **antes do primeiro build** (T003): Restrições Técnicas passam a citar BepInEx 6 be.791 + patcher e o namespace real dos tipos do jogo; o princípio II passa a citar `BepInEx\plugins`; o princípio VI deixa de citar o bloqueio do MelonLoader.

## Project Structure

### Documentation (this feature)

```text
specs/004-migracao-bepinex/
├── plan.md
├── research.md
├── quickstart.md
└── tasks.md
```

Sem `data-model.md` (não há dados novos) e sem `contracts/` (a interface para o jogador é o guia do README).

### Source Code (repository root)

```text
BetterMoonSweeper.csproj        # referências BepInEx; deploy em BepInEx\plugins
src/
├── BetterMoonSweeperMain.cs    # BasePlugin + Log estático + Harmony.PatchAll
├── GravityBootsHotkey.cs       # logger novo
├── FactoryTree.cs              # logger novo
├── Patches.cs                  # + Postfix MoonWorld.Update → GravityBootsHotkey.Tick
└── Upgrades.cs                 # usings
```

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Princípio III: o primeiro teste em jogo grava níveis `jonasmod.*` no save nativo (Steam Cloud) | Herdado da v1 (spec 001, FR-009, não implementado); o teste em jogo é o objetivo desta feature | Implementar o save separado antes exige o mod rodando para testar. Mitigação: backup completo de `LocalLow\DragDropGames\MoonSweepers` (T006), teste só numa sessão nova descartável (T015) e conferência de que ela abre sem o mod (T021) |
| Dependência de um binário não auditado (patcher do Nexus) | Única rota pronta que os outros mods usam; escolha do usuário | A build aberta (projeto Mahjong) exige compilar 3 projetos e distribuir a build para os amigos; fica como plano B |
