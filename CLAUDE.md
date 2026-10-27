# BetterMoonSweeper — contexto para o Claude

Mod para o jogo Steam **Moon Sweepers** (DragDrop Games), co-op de até 16 jogadores.
O design acordado, a lista de classes do jogo e as instruções de build ficam no [README.md](README.md). Mantenha o README atualizado quando decisões mudarem.

## Ambiente

- Jogo: `C:\Program Files (x86)\Steam\steamapps\common\Moon Sweepers`
- Unity 6000.6.0f1, **IL2CPP** (`GameAssembly.dll` + `MoonSweepers_Data\il2cpp_data\Metadata\global-metadata.dat`, versão de metadata 108). Não há DLLs Mono para editar.
- Nomes de classes e métodos não são ofuscados (Compactor, Kiln, AddMoney, CrewBuildMoney, BuildUpgradesPage, Dev*).
- Loader: MelonLoader v0.7.3 (`version.dll` + `MelonLoader\` na pasta do jogo), instalado em 2026-10-27. O loader injeta, mas o Cpp2IL 2022.1.0-pre-release.21 que ele baixa só suporta metadata 23–106 e falha na 108 (o jogo trava esperando Enter no console). A branch `development` do Cpp2IL (commit b5ad444b, 2026-09-12) já suporta 23–108, mas não tem release. Esse build foi colocado em `MelonLoader\Dependencies\Il2CppAssemblyGenerator\Cpp2IL\Cpp2IL.exe`, e a geração das DLLs funciona (`MelonLoader\Il2CppAssemblies`, 109 DLLs), e o BetterMoonSweeper é carregado. **Bloqueio atual:** em runtime, `[Il2CppInterop] Class::Init signatures have been exhausted` seguido de `No Support Module Loaded!`, então nenhum código de mod roda. É o mesmo problema da issue LavaGang/MelonLoader#1159 (Unity 6000.3+); a correção depende do PR LavaGang/MelonLoader#1180 (Il2CppInterop v2), ainda aberto em 2026-10-27. O log fica em `<jogo>\MelonLoader\Latest.log`.
- Log da Unity: `%USERPROFILE%\AppData\LocalLow\DragDropGames\MoonSweepers\Player.log`.
- Saves ficam na Steam Cloud (`LocalLow\...\MoonSessions`).
- Backup do estado anterior ao MelonLoader: `C:\Users\Diuli\Documents\MoonSweepers-backup-2026-10-27`.

## Projeto

- `BetterMoonSweeper.csproj`, net6.0, referencia `MelonLoader\net6\*.dll` e `MelonLoader\Il2CppAssemblies\*.dll` (gerados na primeira execução bem-sucedida do jogo com o loader).
- `dotnet build -c Release` copia `BetterMoonSweeper.dll` para `<jogo>\Mods`.
- Patches com HarmonyX via MelonLoader; tipos do jogo vêm do namespace `Il2CppMoonCleaners` gerado pelo Il2CppInterop.
- O mod se chamava JonasMod até 2026-10-27; a pasta do repositório e os ids internos dos nós da árvore (`jonasmod.*`) mantêm o nome antigo de propósito.
- Specs, planos e tarefas ficam em `specs/` (Spec Kit); regras em `.specify/memory/constitution.md`.

## Regras

- Comunicação com o usuário em português.
- Pedir autorização antes de baixar qualquer coisa ou alterar arquivos na pasta do jogo; fazer backup antes.
- O mod é para co-op com amigos que também instalam o mod; não fazer nada que afete jogadores sem o mod.
- JONAS 2.0 (modelo 3D maior do Jonas) é fase 2; no código o Jonas se chama `Joonas` (`MoonJoonasRig`, `MoonJoonasMotion`, `MoonJoonasGauge`).
