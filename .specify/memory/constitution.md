<!--
Sync Impact Report
- Version change: 1.0.0 → 1.1.0 (MINOR: troca de loader, texto dos princípios II e VI)
- Princípios modificados: II (deploy em `BepInEx\plugins` em vez de `Mods`); VI (removida a
  referência ao bloqueio do MelonLoader)
- Seções modificadas: Restrições Técnicas (BepInEx 6 be.791 + patcher; namespace `MoonCleaners`)
- Seções adicionadas/removidas: nenhuma
- Origem: specs/004-migracao-bepinex
- TODOs: nenhum
-->

# BetterMoonSweeper Constitution

## Core Principles

### I. Co-op só entre jogadores com o mod

- Todo jogador de uma sessão MUST ter o BetterMoonSweeper na mesma versão (`BetterMoonSweeperMain.Version`).
- Nenhuma mudança MAY alterar o que um jogador sem o mod vê, recebe ou salva. Quem não tem o mod
  MUST ser barrado no lobby, nunca jogar "meio-modado".
- Lógica que depende de estado compartilhado (níveis de upgrade, dinheiro do time) MUST dar o
  mesmo resultado no host e nos clientes, ou passar pela autoridade do host já existente no jogo.

Razão: o mod é feito para jogar com amigos que também o instalam; dessincronia em co-op estraga
a sessão de todos.

### II. Instalação do jogo protegida

- Baixar qualquer coisa, ou alterar arquivos na pasta do jogo, MUST ter autorização explícita do
  usuário, e um backup MUST ser feito antes.
- O build MAY copiar apenas `BetterMoonSweeper.dll` para `<jogo>\BepInEx\plugins`. Qualquer outro
  arquivo do jogo é somente leitura para o projeto.

Razão: o jogo é IL2CPP e não há como "recompilar" de volta; um arquivo corrompido exige
reinstalação e pode afetar saves na Steam Cloud.

### III. Save original intocado

- Os dados do mod (níveis dos upgrades do mod) MUST ficar fora do save original do jogo, num
  arquivo próprio no PC do host.
- Remover o mod MUST deixar o save original carregável pelo jogo sem mod.

Razão: os saves ficam na Steam Cloud; um save que só abre com o mod prende o grupo ao mod.

### IV. Patches mínimos e idempotentes

- Preferir Prefix/Postfix do Harmony que ajustam um resultado ou valor a substituir um método
  inteiro.
- Valores originais do jogo MUST ser lidos uma vez e guardados; o mod recalcula a partir do
  original, nunca acumula multiplicadores sobre um valor já alterado.
- Injeções na árvore de upgrades e afins MUST ser idempotentes (rodar duas vezes não duplica nada).
- Cada upgrade novo MUST ser descrito em `src/Upgrades.cs` (id, nome, níveis, preço) e não
  espalhado pelos patches.

Razão: o jogo atualiza sem aviso; patches pequenos quebram menos e são fáceis de revisar.

### V. Símbolos do jogo rastreáveis

- Toda classe, método ou campo do jogo usado pelo mod MUST existir em `docs/assembly-csharp.txt`
  (gerado por `tools/ClassDump`). Depois de uma atualização do jogo, o dump MUST ser regenerado.
- Suposições sobre o comportamento do jogo que ainda não foram vistas em jogo MUST estar
  marcadas como hipótese (comentário no código ou `research.md` da feature).

Razão: o código do jogo não é legível (IL2CPP); só temos assinaturas, não corpos de método.

### VI. Validação em jogo antes de "pronto"

- Uma feature só MAY ser marcada como concluída depois de compilar (`dotnet build -c Release`) e
  de os cenários do `quickstart.md` terem sido executados em jogo, incluindo uma sessão co-op com
  pelo menos 2 jogadores quando a feature mexer em estado compartilhado.
- Enquanto os cenários não forem executados em jogo, a feature fica no máximo em "implementada,
  não testada".

Razão: não há testes automatizados viáveis contra um jogo IL2CPP fechado.

## Restrições Técnicas

- Jogo: Moon Sweepers, Unity 6000.6.0f1, IL2CPP (metadata 108).
- Loader: BepInEx 6.0.0-be.791 (Unity.IL2CPP win-x64) + MoonSweepers BepInEx Patcher (Nexus mod 9)
  + HarmonyX; tipos do jogo vêm do namespace `MoonCleaners` das DLLs geradas em `BepInEx\interop`.
- Projeto: `BetterMoonSweeper.csproj`, net6.0, uma única DLL em `src/`.
- Textos visíveis ao jogador, documentação e comunicação MUST estar em português.
- Nada de dependências NuGet novas sem autorização (regra II).

## Fluxo de Desenvolvimento

- Toda feature nova passa pelo Spec Kit: `specify` → `plan` → `tasks` → `analyze` (com contexto
  fresco) → `implement`. As specs ficam em `specs/NNN-nome/`.
- O `README.md` MUST ser atualizado quando uma decisão de design mudar ou uma feature for
  concluída (tabela de status e de upgrades).
- Valores de balanceamento (preços, multiplicadores) são provisórios até serem testados em jogo e
  MUST ficar todos em `src/Upgrades.cs`.

## Governance

- Esta constituição prevalece sobre preferências de implementação. Um plano que viole um princípio
  MUST justificar a exceção na seção "Complexity Tracking" do `plan.md`.
- Emendas: editar este arquivo, atualizar o Sync Impact Report e a versão (MAJOR remove ou
  redefine princípio; MINOR adiciona princípio ou seção; PATCH ajusta texto).
- Orientações do dia a dia ficam em `CLAUDE.md`; este arquivo só guarda regras.

**Version**: 1.1.0 | **Ratified**: 2026-10-27 | **Last Amended**: 2026-10-27
