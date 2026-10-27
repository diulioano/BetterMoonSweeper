# Implementation Plan: Mais níveis no Contrato Lunar

**Branch**: `003-contrato-lunar-niveis` | **Date**: 2026-10-27 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/003-contrato-lunar-niveis/spec.md`

## Summary

O Contrato Lunar passa de 3 para 6 níveis (+75 / 100 / 150% nos níveis 4 a 6). Como `maxLevel`,
preços, texto do efeito e o patch de `PayoutMultiplier` já derivam do array `Multiplicadores` de
`src/Upgrades.cs`, a mudança de código é estender esse array e subir a versão do mod. Os riscos
estão no jogo (limite de nível na compra, desenho dos marcadores) e têm fallback em
[research.md](research.md).

## Technical Context

**Language/Version**: C# (LangVersion latest), net6.0

**Primary Dependencies**: MelonLoader 0.7.3, HarmonyX, Il2CppInterop, `Il2CppMoonCleaners`

**Storage**: nível em `MoonWorld.treeLevels` pelo fluxo nativo (igual à v1)

**Testing**: manual em jogo, pelo [quickstart.md](quickstart.md); sem testes automatizados

**Target Platform**: Windows, Moon Sweepers (Unity 6000.6, IL2CPP)

**Project Type**: mod (DLL única carregada pelo MelonLoader)

**Performance Goals**: sem mudança; nenhum código novo roda por quadro

**Constraints**: níveis 1 a 3 idênticos à v1; saves da 0.1.0 continuam válidos

**Scale/Scope**: `src/Upgrades.cs` (array e comentário de preços), `src/BetterMoonSweeperMain.cs` (versão), `README.md`, spec 001

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Princípio | Situação |
|---|---|
| I. Co-op só com o mod | ⚠️ Mesmo cálculo no host e nos clientes, e a versão sobe obrigatoriamente; mas a barreira de lobby (spec 001, FR-008) ainda não existe. Ver Complexity Tracking |
| II. Instalação protegida | ✅ Só a DLL |
| III. Save intocado | ⚠️ Herdado da v1 (spec 001, FR-009); esta feature não piora nem melhora |
| IV. Patches mínimos | ✅ Nenhum patch novo; a mudança fica em `Upgrades.cs` |
| V. Símbolos rastreáveis | ✅ `TreeMaxLevel`, `TryPurchaseTreeNode`, `TryGetTreePrice`, `NodeView.pips/max`, `MoonWorld.Roman` no dump; hipóteses H1 a H4 marcadas |
| VI. Validação em jogo | ⚠️ Depende do desbloqueio do MelonLoader |

Reavaliação pós-design: sem mudanças.

## Project Structure

### Documentation (this feature)

```text
specs/003-contrato-lunar-niveis/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── no-contrato-lunar.md
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── Upgrades.cs      # Multiplicadores do ContratoLunar: 3 → 6 valores
└── BetterMoonSweeperMain.cs  # Version
```

**Structure Decision**: projeto único existente; nenhum arquivo novo.

## Design

```text
Upgrades.ContratoLunar.Multiplicadores = { 1.10f, 1.25f, 1.50f, 1.75f, 2.00f, 2.50f }
  → NivelMaximo = 6
  → FactoryTree.CriarNo: maxLevel = 6; prices = 2500 × 3^(n-1); effect = "+{(m-1)*100:0}% de dinheiro"
  → PayoutMultiplierPatch: __result × Multiplicador(nivel), sem mudança
```

- Fallbacks, só se o teste em jogo mostrar problema:
  - **A** (R2): Postfix em `MoonWorld.TreeMaxLevel` para os nós do mod.
  - **B** (R1): nó separado "Contrato Lunar II" com `requires = (jonasmod.contract, 3)`. Exige emendar
    a spec (FR-004) e mudar o `PayoutMultiplierPatch`, por isso só depois de A falhar.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Princípio I: a barreira de lobby ainda não existe | Herdado da v1 (spec 001, FR-008) | A barreira é uma feature própria; até lá o grupo combina a versão fora do jogo, e a subida obrigatória de versão deixa a diferença visível no log |
| Princípio III: o nível vai para o save do jogo | Herdado da v1 (spec 001, FR-009) | Implementar o save separado aqui mistura features; quando ele existir, cobre este nó sem mudança |
