# Spec 006: Armazenamento, receptor de cápsulas e esteiras

**Status**: implementada em 0.5.0; teste em jogo pendente. Pedido: "upgrade nos storages ou receptor de cápsulas" e, depois, "upgrade para a velocidade das esteiras".

## Análise

O jogo guarda as capacidades em propriedades estáticas (docs/assembly-csharp.txt). Mesma técnica do Turbo Compactor: o valor original é lido uma vez e, antes de cada passo de produção (`StepProductionPatch`), a propriedade é regravada como `base × multiplicador do nível` (`src/Armazenamento.cs`). Host e clientes têm o mod e os mesmos níveis. Nada vai para o save.

| Upgrade | Propriedade | Níveis | Preço base |
|---|---|---|---|
| Bigger Catcher (receptor de cápsulas) | `MoonWorld.CatcherStoreKg` | +50% / +100% / +200% | 2.000 |
| Bigger Silo (silo da garagem) | `MoonWorld.GarageSiloCapacityKg` | +50% / +100% / +200% | 2.000 |
| Turbo Belts (esteiras) | `MoonWorld.BeltSpeed` | +25% / +50% / +100% | 1.500 |

Preços ×3 por nível e porcentagens são provisórios (decididos "por cima", a pedido do usuário).

## Riscos

- Se uma propriedade for constante inlinada pelo IL2CPP, escrever nela não muda nada. O log mostra `Capacidade <nome>: base=X → Y`; conferir em jogo (quickstart).
- Falha em uma propriedade é registrada uma vez e não derruba as outras.
- A velocidade das esteiras pode afetar a física das cápsulas; começar com ganhos modestos.

## Removido

- Bigger Station (`MoonCollector.Capacity`) saiu na 0.5.1 a pedido do usuário: efeito não confirmado.

## Retirada (0.5.2)

Gravar `MoonWorld.CatcherStoreKg` (base 100) derrubou o jogo com AccessViolationException: a propriedade é uma constante do jogo, a leitura funciona mas a escrita não. Os upgrades Bigger Catcher, Bigger Silo e Turbo Belts saíram. Caminho possível: patches Harmony nos métodos que usam a capacidade (`CatcherFull`, `DepositCatch`), sem escrever na constante; precisa de teste em jogo.
